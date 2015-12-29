using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Behavior.Choices.Bazaar;
using VoMP.Core.Behavior.Choices.Bonus;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    public class GenerateResourcesBehavior
    {
        public static IActionChoice GenerateVp(AiState state)
        {
            state.Shortfall = new Cost {Vp = 3};
            var cityActions = state.GetValidCityActions(ResourceType.Vp);
            var bestAction = state.ChooseBestAction(cityActions, (r, c) => r.Vp >= c.Vp);
            return bestAction;
        }

        public static IActionChoice GenerateResources(AiState state, Cost cost, string reason)
        {
            if (!state.AvailableDice.Any()) return null;
            var player = state.Player;

            // Track shortfall in state to be used later
            var shortfall = state.GetShortfall(cost);
            state.Shortfall = shortfall;
            if (shortfall.Rating == 0)
                throw new InvalidOperationException("No resources need to be generated.");

            player.Debug($"needs to generate {shortfall} in order to {reason}");

            if (shortfall.Gold > 0)
            {
                var gold = GenerateGold(state);
                if (gold != null) return gold;
            }
            if (shortfall.Silk > 0)
            {
                var silk = GenerateSilk(state);
                if (silk != null) return silk;
            }
            if (shortfall.Pepper > 0)
            {
                var pepper = GeneratePepper(state);
                if (pepper != null) return pepper;
            }
            if (shortfall.Camel > 0)
            {

                var khansFavor = UseKhansFavor(state);
                var camelBazaar = VisitBazaar(state, new CamelBazaar(state.Player));

                var cityActions = state.GetValidCityActions(ResourceType.Camel);
                var bestAction = state.ChooseBestAction(cityActions.Concat(new[] {khansFavor, camelBazaar}), (r, c) => r.Camel >= c.Camel);
                if (bestAction != null) return bestAction;
            }
            if (shortfall.Coin > 0)
            {
                var cityActions = state.GetValidCityActions(ResourceType.Coin);
                var takeFiveCoins = TakeFiveCoins(state);
                var bestAction = state.ChooseBestAction(cityActions.Concat(new ISpaceActionChoice[] {takeFiveCoins}), (r, c) => r.Coin >= c.Coin);
                if (bestAction != null) return bestAction;

                // Prevent from using money bag after action has been taken to prevent it from being used
                // when Take 5 Coins is available but can't be used this turn
                if (player.HasTakenActionThisTurn) return null;
                return new MoneyBag(player, player.Game.MoneyBagSpace, state.AvailableDice.GetLowestDie());
            }
            if (shortfall.Vp > 0)
            {
                var cityActions = state.GetValidCityActions(ResourceType.Vp);
                var bestAction = state.ChooseBestAction(cityActions, (r, c) => r.Vp >= c.Vp);
                if (bestAction != null) return bestAction;
            }
            return null;
        }

        private static IActionChoice GenerateGold(AiState state)
        {
            var khansFavor = UseKhansFavor(state);
            var goldBazaar = VisitBazaar(state, new GoldBazaar(state.Player));
            var cityActions = state.GetValidCityActions(ResourceType.Gold);
            var bestAction = state.ChooseBestAction(cityActions.Concat(new[] {khansFavor, goldBazaar}), (r, c) => r.Gold >= c.Gold);
            return bestAction;
        }

        private static IActionChoice GenerateSilk(AiState state)
        {
            var khansFavor = UseKhansFavor(state, ResourceType.Silk);
            var silkBazaar = VisitBazaar(state, new SilkBazaar(state.Player));
            var cityActions = state.GetValidCityActions(ResourceType.Silk);
            var bestAction = state.ChooseBestAction(cityActions.Concat(new[] { khansFavor, silkBazaar }), (r, c) => r.Silk >= c.Silk);
            return bestAction;
        }

        private static TakeFiveCoins TakeFiveCoins(AiState state)
        {
            var takeFiveCoins = new TakeFiveCoins(state.Player);
            if (!takeFiveCoins.IsValid()) return null;
            var lowestDie = state.GetDiceAvailableFor(takeFiveCoins.Space).GetLowestDie();
            if (lowestDie == null) return null;
            takeFiveCoins.Die = lowestDie;
            var takeFiveCoinsCost = lowestDie.HasValue ? takeFiveCoins.GetCost() : new Cost {Coin = 1};
            if (takeFiveCoinsCost.Coin > 2 || !state.PlayerCanPay(takeFiveCoinsCost))
                takeFiveCoins = null;
            return takeFiveCoins;
        }

        private static IActionChoice GeneratePepper(AiState state)
        {
            var khansFavor = UseKhansFavor(state, ResourceType.Pepper);
            var pepperBazaar = VisitBazaar(state, new PepperBazaar(state.Player));
            var cityActions = state.GetValidCityActions(ResourceType.Pepper);
            var bestAction = state.ChooseBestAction(cityActions.Concat(new[] {khansFavor, pepperBazaar}), (r, c) => r.Pepper >= c.Pepper);
            return bestAction;
        }

        public static ISpaceActionChoice VisitBazaar(AiState state, BazaarBase bazaar)
        {
            var count = bazaar.Space.RequiredDice;
            if (!bazaar.IsValid() || state.AvailableDice.Count < count) return null;
            bazaar.Value = state.GetBestDiceValue(count);
            bazaar.Dice = state.AvailableDice.GetLowestDice(count, bazaar.Value);
            var occCost = state.GetOccupancyCost(bazaar);
            if (bazaar.Value >= 4 && state.PlayerCanPay(occCost)) return bazaar;
            return null;
        }

        public static ISpaceActionChoice UseKhansFavor(AiState state, ResourceType resourceType = ResourceType.Gold)
        {
            var player = state.Player;
            var khansFavor = new KhansFavor(player) {ResourceType = resourceType};
            if (!khansFavor.IsValid()) return null;

            var availableDice = state.GetDiceAvailableFor(khansFavor.Space);
            var die = availableDice.GetLowestDie(khansFavor.MinimumValue);
            if (die == null) return null;

            khansFavor.Die = die;
            return khansFavor;
        }

    }
}