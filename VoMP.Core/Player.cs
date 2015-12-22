using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using VoMP.Core.Actions;
using VoMP.Core.Behavior;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Extensions;

namespace VoMP.Core
{
    public class Player
    {
        private readonly int _startingPosition;
        private readonly IBehavior _behavior;
        private Location _pawnLocation;

        private Player(Game game, Color color, int startingPosition)
        {
            Color = color;
            Game = game;
            _startingPosition = startingPosition;
            _behavior = new AiBehavior();
        }

        public List<Die> AvailableDice { get; private set; }

        public Game Game { get; }

        public Color Color { get; set; }
        public int Vp { get; set; }

        public ResourceBag Resources { get; private set; }
        public List<Contract> CompletedContracts { get; } = new List<Contract>();
        public ISet<Location> TradingPosts { get; } = new HashSet<Location>();
        public bool HasAvailableDice => AvailableDice.Count > 0;
        public List<Goal> Goals { get; set; }

        public List<Contract> Contracts { get; } = new List<Contract>();
        public bool HasBoughtBlackDieThisTurn { get; set; }
        public RouteMap RouteMap { get; private set; } = RouteMap.Standard();

        public bool HasTakenActionThisTurn { get; set; }

        public static Player Create(Game game, Color color, int startingOrder)
        {
            var player = new Player(game, color, startingOrder)
            {
                Resources = new ResourceBag
                {
                    Camel = 2,
                    Coin = 6 + startingOrder
                }
            };
            player.SetPawnLocation(Location.Venezia);
            return player;
        }

        private void SetPawnLocation(Location location)
        {
            MovePawn(_pawnLocation, location);
        }

        public void StartRound()
        {
            // Award bonuses
            GetTradingPostBonusSpaces().ForEach(ms => ms.GrantCityBonusTo(this));
            // Roll dice and grant compensation
            AvailableDice = Enumerable.Range(0, 5).Select(x => new Die(Color)).OrderBy(d => d.Value).ToList();
            Output($"rolls {AvailableDice.Select(x => x.Value).ToDelimitedString()}");
            var compensation = Math.Max(0, 15 - AvailableDice.Sum(x => x.Value));
            if (compensation <= 0) return;
            GainReward(ChooseCamelOrCoin(compensation), "compensation");
        }

        public IEnumerable<MapLocation> GetTradingPostBonusSpaces()
        {
            return TradingPosts.Select(tp => Game.GetMapSpace(tp)).Where(ms => ms.CityBonus != null);
        }

        public bool CanPay(Cost cost)
        {
            return Resources.CanPay(cost);
        }

        public void GainContract(Contract contract)
        {
            if (Contracts.Count >= 2)
                DiscardContract();
            Contracts.Add(contract);
        }

        private void DiscardContract()
        {
            var discard = _behavior.ChooseContractToDiscard(this);
            Output($"discards contract for {discard}");
            Contracts.Remove(discard);
            Game.DiscardContract(discard);
        }

        public void GainReward(Reward reward, string sourceDescription)
        {
            Resources = Resources.Add(reward.GetResources());
            Output($"gains {reward} from {sourceDescription} - {Resources}");
            if (reward.OtherCityBonus > 0)
            {
                var cityBonus = _behavior.ChooseOtherCityBonus(this);
                GainReward(cityBonus.Reward, $"{cityBonus.Reward} city bonus");
            }
            if (reward.TradingPostBonus > 0)
            {
                var cityBonuses = _behavior.ChooseTradingPostBonuses(this, reward.TradingPostBonus);
                foreach (var bonus in cityBonuses)
                {
                    GainReward(bonus.Reward, $"{bonus.Reward} trading post bonus");
                }
            }
            if (reward.Contract > 0)
                DrawBonusContract();
            if (reward.Die > 0)
                GainBlackDie();
            if (reward.Good > 0)
                GainReward(_behavior.ChooseGoodsToGain(this, reward.Good), "gaining goods");
            if (reward.Move > 0)
            {
                var path = _behavior.GetMovePath(this, reward.Move);
                if (path == null) return;
                var cost = path.GetCost();
                if (CanPay(cost))
                    Move(path);
                else
                    Output($"does not have {cost} needed to move to {path.Last().End}");
            }
        }

        public void GainBlackDie()
        {
            if (Game.BlackDice == 0)
            {
                Game.Output("No Black Dice are left to gain");
                return;
            }
            Game.BlackDice--;
            var die = new Die(Color.Black);
            Output($"rolls {die}");
            AvailableDice.Add(die);
        }

        private void DrawBonusContract()
        {
            if (Contracts.Count >= 2)
                DiscardContract();
            var contract = Game.DrawBonusContract();
            Output($"gains contract for {contract}");
            Contracts.Add(contract);
        }

        public void TakeTurn()
        {
            HasBoughtBlackDieThisTurn = false;
            HasTakenActionThisTurn = false;
            while (AvailableDice.Any())
            {
                var validActions = GetValidActions().ToList();
                var choice = _behavior.ChooseAction(this, validActions);
                if (choice == null)
                {
                    if (HasTakenActionThisTurn) break;
                    choice = new MoneyBag(this, Game.MoneyBagSpace, AvailableDice.First());
                }
                choice.Execute();
            }
        }

        private IEnumerable<IAction> GetValidActions()
        {
            return Game.Actions.Where(a => a.IsValid(this));
        }

        public bool HasTradingPost(Location location)
        {
            return TradingPosts.Contains(location);
        }

        public Location GetPawnLocation()
        {
            return _pawnLocation;
        }

        public void TakeDice(IEnumerable<Die> dice)
        {
            dice.ForEach(x => AvailableDice.Remove(x));
        }

        public void TakeDie(Die die)
        {
            TakeDice(new[] {die});
        }

        public void PayCost(Cost cost, string sourceDescription)
        {
            Output($"pays {cost} to {sourceDescription}");
            if (cost.Good > 0)
                cost = _behavior.ChooseGoodsToPay(this, cost);
            Resources = Resources.Subtract(cost);
        }

        public void MovePawn(Location start, Location end)
        {
            if (start != Location.Unknown)
                Game.GetMapSpace(start).RemovePawn(Color);
            _pawnLocation = end;
            Game.GetMapSpace(end).AddPawn(Color);
        }

        public void Move(List<Route> path)
        {
            var cost = path.GetCost();
            var start = path.First().Start;
            var end = path.Last().End;
            PayCost(cost, $"move {path.Count} from {start} to {end}");
            MovePawn(start, end);
            if (end.IsTradeLocation())
            {
                TradingPosts.Add(end);
                if (TradingPosts.Count == 8)
                    GainReward(new Reward { Vp = 5 }, "building 8th trading post");
                if (TradingPosts.Count == 9)
                    GainReward(new Reward { Vp = 10 }, "building 9th trading post");
                var endSpace = Game.GetMapSpace(end);
                endSpace.TradingPosts.Add(Color);
                if (endSpace.OutpostBonus != null)
                    endSpace.ClaimOutpostBonus(this);
                if (endSpace.CityBonus != null)
                    endSpace.GrantCityBonusTo(this);
            }
        }

        public void Output(string s)
        {
            Game.Output($"{Color,-6}: {s}");
        }

        public List<Route> GetBestPath()
        {
            // stop traveling if all trading posts have been build
            if (TradingPosts.Count >= 9) return null;
            var pawnAt = GetPawnLocation();
            var goalCities = Goals.SelectMany(g => new[] {g.Location1, g.Location2});
            var targetCities = goalCities.Concat(new[] {Location.Beijing}).Except(TradingPosts).ToList();
            if (targetCities.Any())
                return RouteMap.BestPath(pawnAt, targetCities);
            // continue building trading posts once goal cities have been reached
            var noTradingPost = Locations.All.Where(l => l.IsTradeLocation()).Except(TradingPosts).ToList();
            return noTradingPost.Select(t => RouteMap.ShortestPath(pawnAt, t)).OrderBy(p => p.Count).First();
        }

        public void PlayDice(IList<Die> dice, SpaceAction action)
        {
            Output($"plays {dice.ToDelimitedString("")} on {action}");
            var cost = action.OccupancyCost(dice);
            if (cost.Coin > 0)
                PayCost(cost, $"play on occupied {action} space");

            TakeDice(dice);
            action.Dice.AddRange(dice);
        }

        public Reward ChooseCamelOrCoin(int count)
        {
            return _behavior.ChooseCamelOrCoin(this, count);
        }

        public List<Die> GetDiceAvailableFor(SpaceAction space)
        {
            var availableDice = AvailableDice;
            var playerColorAlreadyPlayedInSpace = space.Dice.Any(x => x.Color == Color);
            if (playerColorAlreadyPlayedInSpace)
                availableDice = availableDice.Where(d => d.Color != Color).ToList();
            return availableDice;
        }

        public bool CanPlayInActionSpace(SpaceAction space)
        {
            // A player can only take one space action per turn
            if (HasTakenActionThisTurn) return false;
            // The player needs to have enough dice left
            var availableDice = GetDiceAvailableFor(space);
            return availableDice.Count >= space.RequiredDice;
        }

        public Cost GetOccupancyCost(SpaceAction space, IEnumerable<Die> dice)
        {
            return space.IsOccupied ?  new Cost {Coin =  dice.MinValue()} : Cost.None;
        }

        public bool HasTradingPostBonusFor(ResourceType resourceType)
        {
            return GetTradingPostBonusSpaces().Any(x => x.CityBonus.Reward.CanReward(resourceType));
        }
    }
}