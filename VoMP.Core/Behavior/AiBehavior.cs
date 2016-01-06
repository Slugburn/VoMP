using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Behavior.Choices.Bazaar;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    public class AiBehavior : IBehavior
    {
        public AiState State { get; set; }

        public IActionChoice ChooseAction(Player player)
        {
            State = new AiState(player, GetBestPath(player));

            // Try to move
            var choice = MakeChoice(player);
            var spaceActionChoice = choice as ISpaceActionChoice;
            if (spaceActionChoice != null && !spaceActionChoice.Dice.All(x=>x.HasValue))
            {
                spaceActionChoice.Dice.ForEach(d=>d.Assign(spaceActionChoice.Value));
            }

            return choice;
        }

        private IActionChoice MakeChoice(Player player)
        {
            if (player.Contracts.Count == 0)
            {
                using (State.ReserveResources(State.GetOutstandingCosts(), "move"))
                {
                    // Try to get more contracts
                    var takeContracts = TakeContractsBehavior.TakeContracts(State, c => true);
                    if (takeContracts != null) return takeContracts;
                }
            }

            // Move
                var move = MoveBehavior.Move(State);
            if (move != null) return move;

            // Try to complete contracts
            var completeContract = CompleteContractBehavior.CompleteContract(State, c => c.Reward.Move == 0 || player.CreatesTradingPostsWhileMoving);
            if (completeContract != null) return completeContract;

            using (State.ReserveResources(State.GetOutstandingCosts(), "move and complete contracts"))
            {
                // Score VP
                var generateVp = GenerateResourcesBehavior.GenerateResources(State, Cost.Of.Vp(4), "score VP");
                if (generateVp != null) return generateVp;

                // Try to get more contracts
                if (player.Contracts.Count == 0)
                {
                    var takeContracts = TakeContractsBehavior.TakeContracts(State, c => true);
                    if (takeContracts != null) return takeContracts;
                }

                // Low hanging fruit
                var lowHanging = PickLowHangingFruit(State);
                return lowHanging ?? GenerateResourcesBehavior.UseMoneyBag(State);
            }

        }

        private static IActionChoice PickLowHangingFruit(AiState state)
        {
            var blackDie = ImproveDiceBehavior.BuyBlackDie(state);
            if (blackDie != null) return blackDie;

            var khansFavor = GenerateResourcesBehavior.UseKhansFavor(state);
            if (khansFavor != null) return khansFavor;

            var pepperBazaar = GenerateResourcesBehavior.VisitBazaar<PepperBazaar>(state).OrderByDescending(a=>a.Value).FirstOrDefault();
            if (pepperBazaar != null && pepperBazaar.Cost.Coin == 0) return pepperBazaar;

            var camelBazaar = GenerateResourcesBehavior.VisitBazaar<CamelBazaar>(state).OrderByDescending(a=>a.Value).FirstOrDefault();
            if (camelBazaar != null && camelBazaar.Cost.Coin == 0) return camelBazaar;

            return null;
        }

        public List<Route> GetMovePath(Player player, int distance)
        {
            var bestPath = GetBestPath(player);
            return (bestPath?.Take(distance))?.ToList();
        }

        public CityBonus ChooseOtherCityBonus(Player player)
        {
            var cityBonuses = CityBonus.CreateAll();
            var shortfall = State.GetOutstandingShortfall();
            if (shortfall.Gold > 0 || shortfall.Silk > 0)
                return cityBonuses.Single(x => x.Reward.UniqueGood > 0);
            if (shortfall.Coin > 3)
                return cityBonuses.Single(x => x.Reward.Coin == 5);
            if (shortfall.Camel > 1)
                return cityBonuses.Single(x => x.Reward.Camel == 3);
            if (shortfall.Camel > 0)
                return cityBonuses.Single(x => x.Reward.Camel == 1 && x.Reward.Coin == 3);
            return cityBonuses.Single(x => x.Reward.Vp == 3);
        }

        public Reward ChooseCamelOrCoin(Player player, int count)
        {
            var shortfall = State?.GetOutstandingShortfall();
            if (shortfall == null || shortfall.Camel > 0 || shortfall.Coin == 0) return Reward.Of.Camel(count);
            return Reward.Of.Coin(count);
        }

        public Reward ChooseGoodsToGain(Player player, int count)
        {
            var shortfall = State.GetOutstandingShortfall();
            if (shortfall == null) return Reward.Of.Gold(count);
            var gold = Math.Min(shortfall.Gold, count);
            count -= gold;
            var silk = Math.Min(shortfall.Silk, count);
            count -= silk;
            var pepper = Math.Min(shortfall.Pepper, count);
            count -= pepper;
            gold += count;
            return Reward.Of.Gold(gold).And.Silk(silk).And.Pepper(pepper);
        }

        public Reward ChooseUniqueGoodsToGain(Player player, int count)
        {
            if (count != 2)
                throw new ArgumentException("Expected value of 2", nameof(count));
            if (player.Contracts.Count == 0) return Reward.Of.Gold(1).And.Silk(1);

            var neededToComplete = player.Contracts.Select(c=>c.Cost).Total();
            var shortfall = player.Resources.GetShortfall(neededToComplete);
            if (shortfall.Silk == 0  && shortfall.Pepper > 0) return Reward.Of.Gold(1).And.Pepper(1);
            if (shortfall.Gold == 0 && shortfall.Pepper > 0) return Reward.Of.Silk(1).And.Pepper(1);
            return Reward.Of.Gold(1).And.Silk(1);
        }

        public Contract ChooseContractToDiscard(Player player)
        {
            // discard most difficult contract to complete
            return player.Contracts.OrderByDescending(c=>player.Resources.GetShortfall(c.Cost).Rating).First();
        }

        public Cost ChooseGoodsToPay(Player player,  Cost cost)
        {
            return player.Resources.ActualizeGoodCost(cost);
        }

        public IEnumerable<CityBonus> ChooseTradingPostBonuses(Player player, int count)
        {
            var bonusSpaces = player.GetTradingPostBonusSpaces().ToList();
            if (count >= bonusSpaces.Count)
                return bonusSpaces.Select(x => x.CityBonus);
            else
                return bonusSpaces.Take(count).Select(x => x.CityBonus);
        }

        public static List<Route> GetBestPath(Player player)
        {
            // stop traveling if all trading posts have been built
            if (player.TradingPosts.Count >= player.MaxTradingPosts) return null;
            var pawnAt = player.GetPawnLocation();
            var goalCities = player.Objectives.SelectMany(g => new[] {g.Location1, g.Location2});
            var targetCities = goalCities.Concat(new[] {Location.Beijing}).Except(player.TradingPosts).ToList();
            if (targetCities.Any())
                return player.RouteMap.BestPath(pawnAt, targetCities);
            // continue building trading posts once goal cities have been reached
            var noTradingPost = Locations.All.Where(l => l.IsTradeCity()).Except(player.TradingPosts).ToList();
            return noTradingPost.Select(t => player.RouteMap.ShortestPath(pawnAt, t)).OrderBy(p => p.Count).First();
        }
    }
}