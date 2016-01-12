using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoMP.Core.Actions;
using VoMP.Core.Characters;
using VoMP.Core.CityCards;
using VoMP.Core.Extensions;

namespace VoMP.Core
{
    public class Game
    {
        private readonly IOutput _output = new ConsoleOutput();
        private List<Contract> _contracts;
        private Player _currentPlayer;
        private Dictionary<Location, MapLocation> _mapLocations;
        private List<Player> _players;
        public List<Contract> AvailableContracts { get; private set; }
        public Player StartPlayer { get; set; }
        public List<IAction> Actions { get; private set; }
        public int Round { get; set; }
        public int BlackDice { get; set; }

        public void SetUp(IEnumerable<ICharacter> characters)
        {
            // Create map spaces
            _mapLocations = Locations.All.ToDictionary(loc => loc, loc => new MapLocation(loc));

            // Assign random city bonuses to small cities
            Locations.SmallCities.PairWithRandom(CityBonus.CreateAll())
                .ForEach(t => _mapLocations[t.Item1].CityBonus = t.Item2);

            // Assign random outpost bonuses to large cities
            Locations.LargeCities.PairWithRandom(OutpostBonus.CreateAll())
                .ForEach(t => _mapLocations[t.Item1].OutpostBonus = t.Item2);

            // Assign random city cards to large cities (3 to Sumatra)
            var cityCardLocations = Locations.LargeCities.Concat(new[] {Location.Sumatra, Location.Sumatra}).ToList();
            cityCardLocations.PairWithRandom(CityCard.CreateAll())
                .ForEach(t => _mapLocations[t.Item1].Actions.Add(new LargeCityAction(t.Item1, t.Item2)));

            // Create list of action spaces
            Actions = _mapLocations.Values.SelectMany(l => l.Actions)
                .Concat(new IAction[]
                {
                    new MoneyBagSpace(), new TakeFiveCoinsSpace(), new KhansFavorSpace(), new GoldBazaarSpace(), new SilkBazaarSpace(), new PepperBazaarSpace(), new CamelBazaarSpace(),
                    new TakeContractSpace(), new TravelSpace()
                })
                .ToList();

            // Shuffle contracts
            _contracts = Contract.CreateContracts().Shuffle();
            AvailableContracts = _contracts.Draw(6);

            // Create players
            _players =
                new[] {Color.Blue, Color.Green, Color.Red, Color.Yellow}.Shuffle()
                    .Select((c, idx) => Player.Create(this, c, idx + 1))
                    .ToList();
            StartPlayer = _players[0];

            // Give each player a starting contract
            _players.PairWithRandom(Contract.CreateStartingContracts()).ForEach(x => x.Item1.GainContract(x.Item2));

            // Choose characters
            _players.PairWith(characters).ForEach(x=>x.Item1.ClaimCharacter(x.Item2));

            // Set pawn location
            _players.ForEach(p=>p.SetStartLocation());

            // Give each player two objectives
            var goalGroups = Objective.CreateAll().Shuffle().Segment(2);
            _players.PairWithRandom(goalGroups).ForEach(x => x.Item1.Objectives = x.Item2.ToList());
        }

        public void StartGame()
        {
            Round = 1;
            do
            {
                StartRound();
                TakeTurns();
                EndRound();
            } while (Round <= 5);
            _output.Write("-- Score --");
            _players.ForEach(p => p.ScoreEndOfGame());
        }

        public IEnumerable<Player> GetPlayers()
        {
            return _players;
        }

        private void StartRound()
        {
            _output.Write($"-- Round {Round} --");
            _players.ForEach(p => p.StartRound());
            _currentPlayer = StartPlayer;
            BlackDice = _players.Count + 1;
        }

        private void TakeTurns()
        {
            while (_players.Any(p => p.HasAvailableDice))
            {
                _currentPlayer.TakeTurn();
                _currentPlayer = GetNextPlayer(_currentPlayer);
            }
        }

        private void EndRound()
        {
            Round++;
            if (Round > 5) return;
            _contracts.AddRange(AvailableContracts);
            AvailableContracts = _contracts.Draw(6);
            // clear played dice
            Actions.OfType<ActionSpace>().ForEach(x => x.ClearDice());
        }

        private Player GetNextPlayer(Player player)
        {
            var nextIdx = _players.IndexOf(player) + 1;
            if (nextIdx >= _players.Count) nextIdx = 0;
            return _players[nextIdx];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            _mapLocations.Values.Select(x => x.ToString()).Where(x => x != null).ForEach(x => sb.AppendLine(x));
            var contracts = AvailableContracts.Select((c, idx) => new {idx = idx + 1, c}).Select(x => $"{x.idx}:{x.c}");
            sb.AppendLine($"Contracts: {contracts.ToDelimitedString(" | ")}");
            return sb.ToString();
        }

        public MapLocation GetMapSpace(Location location)
        {
            return _mapLocations[location];
        }

        public void Output(string s)
        {
            _output.Write(s);
        }

        public void Debug(string s)
        {
            _output.WriteDebug(s);
        }

        public Contract DrawBonusContract()
        {
            return _contracts.Draw();
        }

        public T GetActionSpace<T>() where T:ActionSpace
        {
            return Actions.OfType<T>().SingleOrDefault();
        }

        public void DiscardContract(Contract contract)
        {
            _contracts.Add(contract);
        }

        public IEnumerable<ContractSpace> GetContractSpaces()
        {
            return AvailableContracts.Select((c, idx) => new ContractSpace {Value = idx + 1, Contract = c});
        }

        public MoneyBagSpace MoneyBagSpace { get;  } = new MoneyBagSpace();

        public IEnumerable<MapLocation> GetMapLocations()
        {
            return _mapLocations.Values;
        }
    }
}