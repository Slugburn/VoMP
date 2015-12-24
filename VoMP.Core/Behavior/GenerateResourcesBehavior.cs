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
        public static IActionChoice GenerateResources(AiState state, Cost cost)
        {
            if (!state.AvailableDice.Any()) return null;
            var player = state.Player;

            var shortfall = player.Resources.GetShortfall(cost);
            // Track shortfall in state to be used later
            state.Shortfall = shortfall;

            player.Debug($"needs to generate {shortfall}");

            if (shortfall.Gold > 0)
            {
                var cityAction = CityAction(state, ResourceType.Gold, r => r.Gold);
                if (cityAction != null) return cityAction;

                var khansFavorChoice = UseKhansFavor(state);
                if (khansFavorChoice != null) return khansFavorChoice;
            }
            if (shortfall.Silk > 0)
            {
                var generateSilk = GenerateSilk(state, shortfall);
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
                var bestAction = ChooseBestAction(state, cityActions.Concat(new[] {khansFavor, camelBazaar}), (r, c) => r.Camel > c.Camel);
                if (bestAction != null) return bestAction;
            }
            if (shortfall.Coin > 0)
            {
                var cityAction = CityAction(state, ResourceType.Coin, r => r.Coin);
                if (cityAction != null) return cityAction;

                var takeFiveCoins = new TakeFiveCoins(player);
                if (takeFiveCoins.IsValid())
                {
                    var die = state.GetDiceAvailableFor(takeFiveCoins.Space).GetLowestDie();
                    if (die != null)
                    {
                        takeFiveCoins.Die = die;
                        var occupancyCost = player.GetOccupancyCost(takeFiveCoins.Space, new[] {die});
                        if (occupancyCost.Coin <= 2 && player.CanPay(occupancyCost))
                            return takeFiveCoins;
                    }
                }
                // Prevent from using money bag after action has been taken to prevent it from being used
                // when Take 5 Coins is available but can't be used this turn
                if (player.HasTakenActionThisTurn) return null;
                return new MoneyBag(player, player.Game.MoneyBagSpace, state.AvailableDice.GetLowestDie());
            }
            return null;
        }

        private static IActionChoice ChooseBestAction(AiState state, IEnumerable<ISpaceActionChoice> choices, Func<Reward,Cost,bool> rewardMeetsShortfall)
        {
            var possibleActions = choices.Where(a => a != null).ToList();
            if (!possibleActions.Any()) return null;
            var meetsRequirement = possibleActions.Where(a => rewardMeetsShortfall(a.GetReward(), state.Shortfall)).ToList();
            var canAfford = possibleActions.Where(a => state.PlayerCanPay(a.GetCost())).ToList();
            var best = meetsRequirement.Intersect(canAfford)
                .OrderByDescending(a => a.GetReward().Rating - a.GetCost().Rating)
                .FirstOrDefault();
            return best;
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

        private static IActionChoice GenerateSilk(AiState state, Cost shortfall)
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

        public static IActionChoice CityAction(AiState state, ResourceType resourceType, Func<Reward, int> resourceAccessor)
        {
            var player = state.Player;
            var availableDice = state.AvailableDice;
            var highestValue = availableDice.GetHighestDie().Value;
            var largeCities = GetValidCityActions(state, resourceType).ToList();
            largeCities.Select(c => c.Card).OfType<OptionCityCard>().ForEach(card => card.SelectResouce(player, resourceType));
            var bestCity =
                largeCities.Where(c => highestValue >= c.OptimumValue).OrderBy(c => resourceAccessor(c.GetOptimumReward())).FirstOrDefault();
            if (bestCity == null) return null;
            bestCity.Die = availableDice.GetLowestDie(bestCity.OptimumValue);
            bestCity.Value = bestCity.Die.Value;
            var cost = bestCity.GetCost();
            if (player.CanPay(cost)) return bestCity;
            var generateResources = GenerateMoreResources(state, cost, new[] {bestCity.Die});
            return generateResources;
        }

        private static IActionChoice GenerateMoreResources(AiState state, Cost cost, IList<Die> dice)
        {
            state.Reserve(cost, dice);
            var generateResources = GenerateResources(state, cost);
            if (generateResources == null)
                state.Unreserve(cost, dice);
            return generateResources;
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