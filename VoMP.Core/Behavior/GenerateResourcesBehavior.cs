using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    public class GenerateResourcesBehavior
    {
        public static IActionChoice GenerateResources(AiState state, Cost cost)
        {
            var player = state.Player;

            var validActions = state.ValidActions;
            var shortfall = player.Resources.GetShortfall(cost);
            // Track shortfall in state to be used later
            state.Shortfall = shortfall;
            if (shortfall.Gold > 0)
            {
                var cityAction = CityAction(state, ResourceType.Gold, r => r.Gold);
                if (cityAction != null) return cityAction;

                var khansFavorChoice = UseKhansFavor(state, ResourceType.Gold);
                if (khansFavorChoice != null) return khansFavorChoice;
            }
            if (shortfall.Silk > 0)
            {
                var cityAction = CityAction(state, ResourceType.Silk, r => r.Silk);
                if (cityAction != null) return cityAction;

                var silkChoice = VisitBazaar(player, validActions.Get<SilkBazaarSpace>(), 4);
                if (silkChoice != null) return silkChoice;
                var khansFavorChoice = UseKhansFavor(state, ResourceType.Silk);
                if (khansFavorChoice != null) return khansFavorChoice;
            }
            if (shortfall.Pepper > 0)
            {
                var cityAction = CityAction(state, ResourceType.Pepper, r => r.Pepper);
                if (cityAction != null) return cityAction;
                var pepperChoice = VisitBazaar(player, validActions.Get<PepperBazaarSpace>(), 3);
                if (pepperChoice != null)
                    return pepperChoice;
            }
            if (shortfall.Camel > 0)
            {
                var cityAction = CityAction(state, ResourceType.Camel, r => r.Camel);
                if (cityAction != null) return cityAction;
                var camelChoice = VisitBazaar(player, validActions.Get<CamelBazaarSpace>(), 4);
                if (camelChoice != null)
                    return camelChoice;
            }
            if (shortfall.Coin > 0)
            {
                var cityAction = CityAction(state, ResourceType.Coin, r => r.Coin);
                if (cityAction != null) return cityAction;

                var space = player.Game.GetActionSpace<TakeFiveCoinsSpace>();
                var die = state.GetDiceAvailableFor(space).GetLowest();
                if (die != null)
                {
                    var takeFiveCoins = new TakeFiveCoins(player, space, die);
                    if (takeFiveCoins.IsValid())
                    {
                        var occupancyCost = player.GetOccupancyCost(space, new[] { die });
                        if (occupancyCost.Coin <= 2 && player.CanPay(occupancyCost))
                            return takeFiveCoins;
                    }
                }
                return new MoneyBag(player, player.Game.MoneyBagSpace, state.AvailableDice.GetLowest());
            }
            return null;
        }

        private static List<CityAction> GetValidCityActions(Player player)
        {
            var cityActions = player.TradingPosts.SelectMany(t => player.Game.GetMapSpace(t).Actions)
                .Select(space => new CityAction(player, space))
                .Where(cityAction => cityAction.IsValid())
                .ToList();
            return cityActions;
        }

        public static IActionChoice CityAction(AiState state, ResourceType resourceType, Func<Reward, int> resourceAccessor)
        {
            var player = state.Player;
            var availableDice = state.AvailableDice;
            var highestValue = availableDice.GetHighest().Value;
            var largeCities = GetValidCityActions(player).Where(a=>a.CanGenerate(resourceType));
            var bestCity = largeCities.Where(c => highestValue >= c.OptimumValue).OrderBy(c => resourceAccessor(c.GetOptimumReward())).FirstOrDefault();
            if (bestCity == null) return null;
            bestCity.Die = availableDice.GetLowest(d => d.Value >= bestCity.OptimumValue);
            var cost = bestCity.GetCost();
            return player.CanPay(cost) ? bestCity : GenerateResources(state, cost);
        }

        public static IActionChoice UseKhansFavor(AiState state, ResourceType resourceType)
        {
            var player = state.Player;
            var space = player.Game.GetActionSpace<KhansFavorSpace>();
            var availableDice = state.GetDiceAvailableFor(space);
            var die = availableDice.GetLowest(d => d.Value >= space.MinimumValue);
            var khansFavor = new KhansFavor(player,space,resourceType, die);
            return !khansFavor.IsValid() ? null : khansFavor;
        }

        private static IActionChoice VisitBazaar(Player player, GrandBazaarSpace action, int minValue)
        {
            if (action == null) return null;
            var dice = player.AvailableDice.GetHighest(action.RequiredDice, d => d.Value >= minValue);
            if (dice.Count != action.RequiredDice) return null;

            // generally not a good idea to use an occupied bazaar space unless we have a ton of coins
            if (player.Resources.Coin < action.OccupancyCost(dice).Coin * 2) return null;
            return new GrandBazaar(player, action, dice.MinValue(), dice);
        }
    }
}
