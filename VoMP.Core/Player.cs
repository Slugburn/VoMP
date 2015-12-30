﻿using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Behavior.Choices.Bonus;
using VoMP.Core.Characters;
using VoMP.Core.Extensions;

namespace VoMP.Core
{
    public class Player
    {
        private readonly IBehavior _behavior;
        private readonly int _startingPosition;
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
        public List<Objective> Goals { get; set; }
        public List<Contract> Contracts { get; } = new List<Contract>();
        public bool HasBoughtBlackDieThisTurn { get; set; }
        public RouteMap RouteMap { get; set; } = RouteMap.Standard();
        public bool HasTakenActionThisTurn { get; set; }
        public List<LargeCityAction> CityActions { get; } = new List<LargeCityAction>();
        public List<Action<Player>> CharacterBonuses { get; } = new List<Action<Player>>();

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
            // Create dice
            AvailableDice = Enumerable.Range(0, 5).Select(x => Die.Create(Color)).ToList();

            // Award bonuses
            CharacterBonuses.ForEach(bonus => bonus(this));
            GetTradingPostBonusSpaces().ForEach(ms => ms.GrantCityBonusTo(this));

            if (!RollsDice) return;

            // Roll dice and grant compensation
            AvailableDice.ForEach(d => d.Roll());
            Output($"rolls {AvailableDice.Select(x => x.Value).ToDelimitedString()}");
            var compensation = Math.Max(0, 15 - AvailableDice.Sum(x => x.Value));
            if (compensation <= 0) return;
            GainReward(ChooseCamelOrCoin(compensation), "compensation");
        }

        public bool RollsDice { get; set; } = true;

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
                GainBonusContract();
            if (reward.Die > 0)
                GainBlackDie();
            if (reward.Good > 0)
                GainReward(_behavior.ChooseGoodsToGain(this, reward.Good), "gaining goods");
            if (reward.UniqueGood > 0)
                GainReward(_behavior.ChooseUniqueGoodsToGain(this, reward.UniqueGood), "gaining goods");
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
            GainDie(Color.Black);
        }

        public void GainDie(Color color)
        {
            var die = Die.Create(color);
            AvailableDice.Add(die);
            if (!RollsDice) return;
            die.Roll();
            Output($"rolls {die}");
        }

        public void GainBonusContract()
        {
            if (Contracts.Count >= 2)
                DiscardContract();
            var contract = Game.DrawBonusContract();
            Output($"gains contract for {contract}");
            Contracts.Add(contract);
        }

        public void TakeTurn()
        {
            Debug($"Start Turn - {AvailableDice.ToDelimitedString("")} ({Resources})");
            while (AvailableDice.Any())
            {
                var choice = _behavior.ChooseAction(this);
                if (choice == null)
                {
                    if (HasTakenActionThisTurn) break;
                    choice = new MoneyBag(this, Game.MoneyBagSpace, AvailableDice.GetLowestDie());
                }
                var spaceActionChoice = choice as ISpaceActionChoice;
                if (spaceActionChoice != null && !spaceActionChoice.Dice.All(d=>d.HasValue))
                    throw new UnassignedDieException();
                choice.Execute();
                // notify other players of action choice
                Game.GetPlayers().Except(new[] {this}).ForEach(player=>player.NotifyOfActionChoice(player, choice));
            }
            HasBoughtBlackDieThisTurn = false;
            HasTakenActionThisTurn = false;
        }

        public Action<Player, IActionChoice> NotifyOfActionChoice = (player,choice) => { };

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
            Output(cost.Rating > 0 ? $"pays {cost} to {sourceDescription}" : sourceDescription);
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
            if (end.IsTradeLocation() && !TradingPosts.Contains(end))
            {
                BuildTradingPost(end);
            }
        }

        private void BuildTradingPost(Location end)
        {
            TradingPosts.Add(end);
            if (TradingPosts.Count == 8)
                GainReward(new Reward {Vp = 5}, "building 8th trading post");
            if (TradingPosts.Count == 9)
                GainReward(new Reward {Vp = 10}, "building 9th trading post");
            var endSpace = Game.GetMapSpace(end);
            endSpace.TradingPosts.Add(Color);
            if (endSpace.OutpostBonus != null)
                endSpace.ClaimOutpostBonus(this);
            if (endSpace.CityBonus != null)
                endSpace.GrantCityBonusTo(this);
            CityActions.AddRange(endSpace.Actions);
        }

        public void Output(string s)
        {
            Game.Output($"{Color,-6}: {s}");
        }

        public void Debug(string s)
        {
            if (!HasTakenActionThisTurn)
                Game.Debug($"{Color,-6}: [DEBUG] {s}");
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

        public void PlayDice(IList<Die> dice, ActionSpace space)
        {
            Output($"plays {dice.ToDelimitedString("")} on {space}");
            var cost = GetOccupancyCost(space, dice, 0);
            if (cost.Coin > 0)
                PayCost(cost, $"play on occupied {space} space");

            TakeDice(dice);
            space.PlayDice(dice);
        }

        public Reward ChooseCamelOrCoin(int count)
        {
            return _behavior.ChooseCamelOrCoin(this, count);
        }

        public List<Die> GetDiceAvailableFor(ActionSpace space)
        {
            var availableDice = AvailableDice;
            if (space.ColorAlreadyPlayed(Color))
                availableDice = availableDice.Where(d => d.Color != Color).ToList();
            return availableDice;
        }

        public bool CanPlayInActionSpace(ActionSpace space)
        {
            // A player can only take one space action per turn
            if (HasTakenActionThisTurn) return false;
            // The player needs to have enough dice left
            var availableDice = GetDiceAvailableFor(space);
            return availableDice.Count >= space.RequiredDice;
        }

        public Func<ActionSpace, IEnumerable<Die>, int, Cost> GetOccupancyCost { get; set; } = (space, dice, value) => DefaultGetOccupancyCost(space, dice, value);

        private static Cost DefaultGetOccupancyCost(ActionSpace space, IEnumerable<Die> dice, int value)
        {
            if (!space.IsOccupied) return Cost.None;
            var list = dice as IList<Die> ?? dice.ToList();
            var coin = (list.All(d => d.HasValue) ? list.MinValue() : value);
            return new Cost {Coin = coin};
        }

        public ICharacter Character { get; private set; }

        public bool HasTradingPostBonusFor(ResourceType resourceType)
        {
            return GetTradingPostBonusSpaces().Any(x => x.CityBonus.Reward.CanReward(resourceType));
        }

        public void ScoreEndOfGame()
        {
            var coins = Resources.Coin;
            if (coins >= 10)
            {
                var vpForCoins = coins/10;
                GainReward(new Reward {Vp = vpForCoins}, $"having {coins} coins");
            }
            Goals.ForEach(g =>
            {
                if (HasTradingPost(g.Location1) && HasTradingPost(g.Location2))
                    GainReward(new Reward {Vp = g.Vp}, $"reaching {g.Location1} and {g.Location2}");
            });
            var targets = Goals.SelectMany(g => new[] {g.Location1, g.Location2}).Distinct();
            var targetsReached = targets.Intersect(TradingPosts).ToList();
            var goalScore = new[] {0, 1, 3, 6, 10}[targetsReached.Count];
            GainReward(new Reward {Vp = goalScore}, $"reaching {targetsReached.ToDelimitedString()}");
            if (TradingPosts.Contains(Location.Beijing))
            {
                var index = Game.GetMapSpace(Location.Beijing).TradingPosts.IndexOf(Color);
                var beijingScore = new[] {10, 7, 4, 1}[index];
                var position = new[] {"first", "second", "third", "fourth"}[index];
                GainReward(new Reward {Vp = beijingScore}, $"reaching Beijing {position}");
                var goodCount = Resources.Gold + Resources.Silk + Resources.Pepper;
                if (goodCount > 1)
                {
                    var vpForGoods = goodCount/2;
                    GainReward(new Reward {Vp = vpForGoods}, $"having {goodCount} goods");
                }
            }

            var completedContractCount = CompletedContracts.Count;
            if (completedContractCount == Game.GetPlayers().Max(p => p.CompletedContracts.Count))
                GainReward(new Reward {Vp = 7}, $"completing the most contracts ({completedContractCount})");
        }

        public void ClaimCharacter(ICharacter character)
        {
            Character = character;
            character.ModifyPlayer(this);
        }
    }
}