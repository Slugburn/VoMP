using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Behavior.Choices.Bazaar;
using VoMP.Core.Behavior.Choices.Bonus;
using VoMP.Core.CityCards;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    public class GenerateResourcesBehavior
    {
        public static IActionChoice GenerateVp(AiState state)
        {
            state.Shortfall = new Cost {Vp = 3};
            var cityActions = GetValidCityActions(state, ResourceType.Vp);
            var bestAction = ChooseBestAction(state, cityActions, (r, c) => r.Vp >= c.Vp);
            return bestAction;
        }

        public static IActionChoice GenerateResources(AiState state, Cost cost, string reason)
        {
            if (!state.AvailableDice.Any()) return null;
            var player = state.Player;

            // Track shortfall in state to be used later
            var shortfall = player.Resources.GetShortfall(cost);
            state.Shortfall = shortfall;

            player.Debug($"needs to generate {shortfall} in order to {reason}");

            if (shortfall.Gold > 0)
            {
                var khansFavor = UseKhansFavor(state);
                var cityActions = GetValidCityActions(state, ResourceType.Gold);
                var bestAction = ChooseBestAction(state, cityActions.Concat(new[] {khansFavor}), (r, c) => r.Gold >= c.Gold);
                if (bestAction != null) return bestAction;
            }
            if (shortfall.Silk > 0)
            {
                var generateSilk = GenerateSilk(state);
                if (generateSilk != null) return generateSilk;
            }
            if (shortfall.Pepper > 0)
            {
                var generatePepper = GeneratePepper(state);
                if (generatePepper != null) return generatePepper;
            }
            if (shortfall.Camel > 0)
            {

                var khansFavor = UseKhansFavor(state);
                var camelBazaar = VisitCamelBazaar(state);

                var cityActions = GetValidCityActions(state, ResourceType.Camel);
                var bestAction = ChooseBestAction(state, cityActions.Concat(new[] {khansFavor, camelBazaar}), (r, c) => r.Camel >= c.Camel);
                if (bestAction != null) return bestAction;
            }
            if (shortfall.Coin > 0)
            {
                var cityActions = GetValidCityActions(state, ResourceType.Coin);
                var takeFiveCoins = TakeFiveCoins(state);
                var bestAction = ChooseBestAction(state, cityActions.Concat(new ISpaceActionChoice[] {takeFiveCoins}), (r, c) => r.Coin >= c.Coin);
                if (bestAction != null) return bestAction;

                // Prevent from using money bag after action has been taken to prevent it from being used
                // when Take 5 Coins is available but can't be used this turn
                if (player.HasTakenActionThisTurn) return null;
                return new MoneyBag(player, player.Game.MoneyBagSpace, state.AvailableDice.GetLowestDie());
            }
            if (shortfall.Vp > 0)
            {
                var cityActions = GetValidCityActions(state, ResourceType.Vp);
                var bestAction = ChooseBestAction(state, cityActions, (r, c) => r.Vp >= c.Vp);
                if (bestAction != null) return bestAction;
            }
            return null;
        }

        private static TakeFiveCoins TakeFiveCoins(AiState state)
        {
            var player = state.Player;
            var takeFiveCoins = new TakeFiveCoins(player);
            if (!takeFiveCoins.IsValid()) return null;
            takeFiveCoins.Die = state.GetDiceAvailableFor(takeFiveCoins.Space).GetLowestDie();
            var takeFiveCoinsCost = takeFiveCoins.GetCost();
            if (takeFiveCoinsCost.Coin > 2 || !state.PlayerCanPay(takeFiveCoinsCost))
                takeFiveCoins = null;
            return takeFiveCoins;
        }

        private static IActionChoice ChooseBestAction(AiState state, IEnumerable<ISpaceActionChoice> choices, Func<Reward,Cost,bool> rewardMeetsShortfall)
        {
            var possibleActions = choices.Where(a => a != null && a.IsValid()).ToList();
            if (!possibleActions.Any()) return null;
            var meetsRequirement = possibleActions.Where(a => a.GetReward() != null && rewardMeetsShortfall(a.GetReward(), state.Shortfall)).ToList();
            var canAfford = possibleActions.Where(a => state.PlayerCanPay(a.GetCost())).ToList();
            var best = meetsRequirement.Intersect(canAfford)
                .OrderByDescending(a => a.GetReward().Rating - a.GetCost().Rating)
                .FirstOrDefault();
            if (best != null) return best;
            var bestAffordable = canAfford.OrderByDescending(a => a.GetReward().Rating - a.GetCost().Rating).FirstOrDefault();
            return bestAffordable;
        }

        public static CamelBazaar VisitCamelBazaar(AiState state)
        {
            var player = state.Player;
            var camelBazaar = new CamelBazaar(player);
            if (camelBazaar.IsValid())
            {
                camelBazaar.Dice = state.AvailableDice.GetHighestDice(1);
                camelBazaar.Value = state.AvailableDice.GetHighestDie().Value;
                var occCost = player.GetOccupancyCost(camelBazaar.Space, camelBazaar.Dice);
                if (camelBazaar.Value >= 4 && state.PlayerCanPay(occCost)) return camelBazaar;
            }
            return null;
        }

        private static IActionChoice GeneratePepper(AiState state)
        {
            var khansFavor = UseKhansFavor(state, ResourceType.Pepper);
            var pepperBazaar = VisitPepperBazaar(state);
            var cityActions = GetValidCityActions(state, ResourceType.Pepper);
            var bestAction = ChooseBestAction(state, cityActions.Concat(new[] {khansFavor, pepperBazaar}), (r, c) => r.Pepper >= c.Pepper);
            return bestAction;
        }

        public static ISpaceActionChoice VisitPepperBazaar(AiState state)
        {
            var player = state.Player;
            var pepperBazaar = new PepperBazaar(player);
            if (!pepperBazaar.IsValid()) return null;
            pepperBazaar.Dice = state.AvailableDice.GetHighestDice(1);
            pepperBazaar.Value = state.AvailableDice.GetHighestDie().Value;
            var cost = player.GetOccupancyCost(pepperBazaar.Space, pepperBazaar.Dice);
            if (pepperBazaar.Value < 4) return null;
            return state.PlayerCanPay(cost) ? pepperBazaar : null; // GenerateMoreResources(state, cost, pepperBazaar.Dice);
        }

        private static IActionChoice GenerateSilk(AiState state)
        {
            var khansFavor = UseKhansFavor(state, ResourceType.Silk);
            var silkBazaar = VisitSilkBazaar(state);
            var cityActions = GetValidCityActions(state, ResourceType.Silk);
            var bestAction = ChooseBestAction(state, cityActions.Concat(new[] { khansFavor, silkBazaar }), (r, c) => r.Silk >= c.Silk);
            return bestAction;
        }

        private static ISpaceActionChoice VisitSilkBazaar(AiState state)
        {
            var silkBazaar = new SilkBazaar(state.Player);
            if (silkBazaar.IsValid() && state.AvailableDice.Count >= 2)
            {
                silkBazaar.Value = state.AvailableDice.GetHighestDice(2).MinValue();
                silkBazaar.Dice = state.AvailableDice.GetLowestDice(2, silkBazaar.Value);
                var occCost = state.Player.GetOccupancyCost(silkBazaar.Space, silkBazaar.Dice);
                if (silkBazaar.Value >= 4 && state.PlayerCanPay(occCost)) return silkBazaar;
            }
            return null;
        }

        private static List<CityAction> GetValidCityActions(AiState state, ResourceType resourceType)
        {
            var player = state.Player;
            var cityActions = player.TradingPosts.SelectMany(t => player.Game.GetMapSpace(t).Actions)
                .Where(space => !space.IsOccupied && space.Card.CanGenerate(player, resourceType))
                .SelectMany(space => CreateCityAction(state, space))
                .Where(a => a.IsValid())
                .ToList();
            cityActions.Select(c => c.Card).OfType<OptionCityCard>().ForEach(card => card.SelectResouce(player, ResourceType.Camel));
            return cityActions;
        }

        private static IEnumerable<CityAction> CreateCityAction(AiState state, LargeCityAction space)
        {
            var availableDice = state.GetDiceAvailableFor(space);
            if (!availableDice.Any()) yield break;
            for (var value = 1; value <= availableDice.MaxValue(); value++)
            {
                yield return new CityAction(state.Player, space)
                {
                    Value = value,
                    Die = availableDice.GetLowestDie(value)
                };
            }
        }

        public static ISpaceActionChoice UseKhansFavor(AiState state, ResourceType resourceType = ResourceType.Gold)
        {
            var player = state.Player;
            var khansFavor = new KhansFavor(player) {ResourceType = resourceType};
            if (!khansFavor.IsValid()) return null;

            var availableDice = state.GetDiceAvailableFor(khansFavor.Space);
            var die = availableDice.GetLowestDie(khansFavor.Space.MinimumValue);
            if (die == null) return null;

            khansFavor.Die = die;
            return khansFavor;
        }

    }
}