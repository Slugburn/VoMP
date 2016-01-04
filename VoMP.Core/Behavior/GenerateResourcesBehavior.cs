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
            state.Shortfall = Cost.Of.Vp(3);
            var cityActions = state.GetValidCityActions(ResourceType.Vp);
            var bestAction = state.ChooseBestAction(cityActions, Cost.Of.Vp(3));
            return bestAction;
        }

        public static IActionChoice GenerateResources(AiState state, Cost cost, string reason)
        {
            if (!state.AvailableDice.Any()) return null;
            var player = state.Player;

            // Track shortfall in state to be used later
            var shortfall = state.GetShortfall(cost);
            state.Shortfall = shortfall;

            // Reserve the cost less shortfall
            var reservedCost = cost.Subtract(shortfall);
            state.ReserveResources(reservedCost, reason);

            if (shortfall.Rating == 0)
                throw new InvalidOperationException("No resources need to be generated.");

            player.Debug($"needs to generate {shortfall} in order to {reason}");

            var choices = state.GetValidCityActions()
                .Concat(new[] {ResourceType.Gold, ResourceType.Silk, ResourceType.Pepper}.Select(t => UseKhansFavor(state, t)))
                .Concat(new[]
                {
                    VisitBazaar(state, new GoldBazaar(state.Player)),
                    VisitBazaar(state, new SilkBazaar(state.Player)),
                    VisitBazaar(state, new PepperBazaar(state.Player)),
                    VisitBazaar(state, new CamelBazaar(state.Player))
                })
                .Concat(new[] {TakeFiveCoins(state), UseMoneyBag(state)});
            return state.ChooseBestAction(choices, shortfall);

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
                var bestAction = state.ChooseBestAction(cityActions.Concat(new[] {khansFavor, camelBazaar}), shortfall);
                if (bestAction != null) return bestAction;
            }
            if (shortfall.Coin > 0)
            {
                var cityActions = state.GetValidCityActions(ResourceType.Coin);
                var takeFiveCoins = TakeFiveCoins(state);
                var useMoneyBag = UseMoneyBag(state);
                var bestAction = state.ChooseBestAction(cityActions.Concat(new [] {takeFiveCoins, useMoneyBag}), shortfall);
                if (bestAction != null) return bestAction;
            }
            if (shortfall.Vp > 0)
            {
                var cityActions = state.GetValidCityActions(ResourceType.Vp);
                var bestAction = state.ChooseBestAction(cityActions, shortfall);
                if (bestAction != null) return bestAction;
            }
            return null;
        }

        public static ISpaceActionChoice UseMoneyBag(AiState state)
        {
            // Prevent from using money bag after action has been taken to prevent it from being used
            // when Take 5 Coins is available but can't be used this turn
            return state.Player.HasTakenActionThisTurn ? null : new MoneyBag(state.Player, state.AvailableDice.GetLowestDie());
        }

        private static IActionChoice GenerateGold(AiState state)
        {
            var khansFavor = UseKhansFavor(state);
            var goldBazaar = VisitBazaar(state, new GoldBazaar(state.Player));
            var cityActions = state.GetValidCityActions(ResourceType.Gold);
            var bestAction = state.ChooseBestAction(cityActions.Concat(new[] {khansFavor, goldBazaar}), state.Shortfall);
            return bestAction;
        }

        private static IActionChoice GenerateSilk(AiState state)
        {
            var khansFavor = UseKhansFavor(state, ResourceType.Silk);
            var silkBazaar = VisitBazaar(state, new SilkBazaar(state.Player));
            var cityActions = state.GetValidCityActions(ResourceType.Silk);
            var bestAction = state.ChooseBestAction(cityActions.Concat(new[] { khansFavor, silkBazaar }), state.Shortfall);
            return bestAction;
        }

        private static ISpaceActionChoice TakeFiveCoins(AiState state)
        {
            var takeFiveCoins = new TakeFiveCoins(state.Player);
            if (!takeFiveCoins.IsValid()) return null;
            var lowestDie = state.GetDiceAvailableFor(takeFiveCoins.Space).GetLowestDie();
            if (lowestDie == null) return null;
            takeFiveCoins.Die = lowestDie;
            return state.PlayerCanAfford(takeFiveCoins) ? takeFiveCoins : null;
        }

        private static IActionChoice GeneratePepper(AiState state)
        {
            var khansFavor = UseKhansFavor(state, ResourceType.Pepper);
            var pepperBazaar = VisitBazaar(state, new PepperBazaar(state.Player));
            var cityActions = state.GetValidCityActions(ResourceType.Pepper);
            var bestAction = state.ChooseBestAction(cityActions.Concat(new[] {khansFavor, pepperBazaar}), state.Shortfall);
            return bestAction;
        }

        public static ISpaceActionChoice VisitBazaar(AiState state, BazaarBase bazaar)
        {
            var count = bazaar.Space.RequiredDice;
            if (!bazaar.IsValid() || state.AvailableDice.Count < count) return null;
            bazaar.Value = state.GetBestDiceValue(count);
            bazaar.Dice = state.AvailableDice.GetLowestDice(count, bazaar.Value);
            if (bazaar.Value >= 4 && state.PlayerCanAfford(bazaar)) return bazaar;
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