using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    public class AiBehavior : IBehavior
    {
        public AiState State { get; private set; }

        public IActionChoice ChooseAction(Player player)
        {
            State = new AiState(player);

            // Try to move
            var move = MoveBehavior.Move(State);
            if (move != null) return move;

            State.ClearResourceReserves();

            // Try to complete contracts
            var completeContract = CompleteContractBehavior.CompleteContract(State, c => c.Reward.Move == 0);
            if (completeContract != null) return completeContract;

            // Score VP
            var cityAction = GenerateResourcesBehavior.GenerateVp(State);
            if (cityAction != null)
                return cityAction;

            // Try to get more contracts
            if (player.Contracts.Count == 0)
            {
                var p1 = new ReserveResourcesChoiceParam(()=> TakeContractsBehavior.TakeContracts(State, c => true), "move and complete contracts") {Cost = State.GetOutstandingCosts()};
                var takeContracts = State.MakeChoiceWithReservedResources(p1);
                if (takeContracts != null) return takeContracts;
            }

            // Low hanging fruit
            var p2 = new ReserveResourcesChoiceParam(()=>PickLowHangingFruit(State), "move and complete contracts") {Cost= State.GetOutstandingCosts()};
            return State.MakeChoiceWithReservedResources(p2);
        }

        private static IActionChoice PickLowHangingFruit(AiState state)
        {
            var blackDie = ImproveDiceBehavior.BuyBlackDie(state);
            if (blackDie != null) return blackDie;

            var khansFavor = GenerateResourcesBehavior.UseKhansFavor(state);
            if (khansFavor != null) return khansFavor;

//            if (player.Contracts.Count < 2)
//            {
//                var takeContracts2 = TakeContractsBehavior.TakeContracts(State, c => true);
//                if (takeContracts2 != null) return takeContracts2;
//            }

            var pepperBazaar = GenerateResourcesBehavior.VisitPepperBazaar(state);
            if (pepperBazaar != null && pepperBazaar.GetCost().Coin == 0) return pepperBazaar;

            var camelBazaar = GenerateResourcesBehavior.VisitCamelBazaar(state);
            if (camelBazaar != null && camelBazaar.GetCost().Coin == 0) return camelBazaar;

            return null;
        }

        public List<Route> GetMovePath(Player player, int distance)
        {
            var bestPath = player.GetBestPath();
            return bestPath?.Take(distance).ToList();
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
            var shortfall = State?.Shortfall;
            if (shortfall == null || shortfall.Camel > 0 || shortfall.Coin == 0) return new Reward {Camel = count};
            return new Reward {Coin = count};
        }

        public Reward ChooseGoodsToGain(Player player, int count)
        {
            var shortfall = State.GetOutstandingShortfall();
            if (shortfall == null || shortfall.Gold > 0) return new Reward {Gold = count};
            if (shortfall.Silk > 0) return new Reward {Silk = count};
            if (shortfall.Pepper > 0) return new Reward {Pepper = count};
            return new Reward {Gold = count};
        }

        public Reward ChooseUniqueGoodsToGain(Player player, int count)
        {
            if (count != 2)
                throw new ArgumentException("Expected value of 2", nameof(count));
            if (player.Contracts.Count == 0) return new Reward { Gold = 1, Silk = 1 };

            var neededToComplete = player.Contracts.Select(c=>c.Cost).Total();
            var shortfall = player.Resources.GetShortfall(neededToComplete);
            if (shortfall.Silk == 0  && shortfall.Pepper > 0) return new Reward {Gold = 1, Pepper = 1};
            if (shortfall.Gold == 0 && shortfall.Pepper > 0) return new Reward {Silk = 1, Pepper = 1};
            return new Reward { Gold = 1, Silk = 1 };
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
            if (!bonusSpaces.Any())
                throw new InvalidOperationException("No trading post bonuses available.");
            if (count >= bonusSpaces.Count)
                return bonusSpaces.Select(x => x.CityBonus);
            else
                return bonusSpaces.Take(count).Select(x => x.CityBonus);
        }
    }
}