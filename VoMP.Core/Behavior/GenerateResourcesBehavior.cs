using System;
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
                var camelBazaar = VisitCamelBazaar(state);

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
            var goldBazaar = VisitGoldBazaar(state);
            var cityActions = state.GetValidCityActions(ResourceType.Gold);
            var bestAction = state.ChooseBestAction(cityActions.Concat(new[] {khansFavor, goldBazaar}), (r, c) => r.Gold >= c.Gold);
            return bestAction;
        }

        private static IActionChoice GenerateSilk(AiState state)
        {
            var khansFavor = UseKhansFavor(state, ResourceType.Silk);
            var silkBazaar = VisitSilkBazaar(state);
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
            var takeFiveCoinsCost = takeFiveCoins.GetCost();
            if (takeFiveCoinsCost.Coin > 2 || !state.PlayerCanPay(takeFiveCoinsCost))
                takeFiveCoins = null;
            return takeFiveCoins;
        }

        public static CamelBazaar VisitCamelBazaar(AiState state)
        {
            var player = state.Player;
            var camelBazaar = new CamelBazaar(player);
            if (!camelBazaar.IsValid()) return null;
            camelBazaar.Dice = state.AvailableDice.GetHighestDice(1);
            camelBazaar.Value = state.AvailableDice.GetHighestDie().Value;
            var occCost = player.GetOccupancyCost(camelBazaar.Space, camelBazaar.Dice);
            if (camelBazaar.Value >= 4 && state.PlayerCanPay(occCost)) return camelBazaar;
            return null;
        }

        private static IActionChoice GeneratePepper(AiState state)
        {
            var khansFavor = UseKhansFavor(state, ResourceType.Pepper);
            var pepperBazaar = VisitPepperBazaar(state);
            var cityActions = state.GetValidCityActions(ResourceType.Pepper);
            var bestAction = state.ChooseBestAction(cityActions.Concat(new[] {khansFavor, pepperBazaar}), (r, c) => r.Pepper >= c.Pepper);
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

        private static ISpaceActionChoice VisitSilkBazaar(AiState state)
        {
            var silkBazaar = new SilkBazaar(state.Player);
            if (!silkBazaar.IsValid() || state.AvailableDice.Count < 2) return null;
            silkBazaar.Value = state.AvailableDice.GetHighestDice(2).MinValue();
            silkBazaar.Dice = state.AvailableDice.GetLowestDice(2, silkBazaar.Value);
            var occCost = state.Player.GetOccupancyCost(silkBazaar.Space, silkBazaar.Dice);
            if (silkBazaar.Value >= 4 && state.PlayerCanPay(occCost)) return silkBazaar;
            return null;
        }

        private static ISpaceActionChoice VisitGoldBazaar(AiState state)
        {
            var goldBazaar = new GoldBazaar(state.Player);
            if (!goldBazaar.IsValid() || state.AvailableDice.Count < 3) return null;
            goldBazaar.Value = state.AvailableDice.GetHighestDice(3).MinValue();
            goldBazaar.Dice = state.AvailableDice.GetLowestDice(3, goldBazaar.Value);
            var occCost = state.Player.GetOccupancyCost(goldBazaar.Space, goldBazaar.Dice);
            if (goldBazaar.Value >= 4 && state.PlayerCanPay(occCost)) return goldBazaar;
            return null;
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