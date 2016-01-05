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
        public static IActionChoice GenerateResources(AiState state, Cost cost, string reason)
        {
            if (!state.AvailableDice.Any()) return null;
            var player = state.Player;

            // Track shortfall in state to be used later
            Cost shortfall;
            if (cost.Vp == 0)
                shortfall = state.GetShortfall(cost);
            else
                shortfall = cost;

            // Reserve the cost less shortfall
            var reservedCost = cost.Subtract(shortfall);

            state.Shortfall = shortfall;
            using (state.ReserveResources(reservedCost, reason))
            {
                if (shortfall.Rating == 0)
                    return null;
                    //throw new InvalidOperationException("No resources need to be generated.");

                player.Debug($"needs to generate {shortfall} in order to {reason}");

                var choices = state.GetValidCityActions().Cast<IActionChoice>()
                    .Concat(player.Contracts.Select(c => new CompleteContract(player, c)))
                    .Concat(new[] { ResourceType.Gold, ResourceType.Silk, ResourceType.Pepper }.Select(t => UseKhansFavor(state, t)))
                    .Concat(VisitBazaar<GoldBazaar>(state))
                    .Concat(VisitBazaar<SilkBazaar>(state))
                    .Concat(VisitBazaar<PepperBazaar>(state))
                    .Concat(VisitBazaar<CamelBazaar>(state))
                    .Concat(new[] { TakeFiveCoins(state), UseMoneyBag(state) });
                return state.ChooseBestAction(choices, shortfall);
            }
        }

        public static ISpaceActionChoice UseMoneyBag(AiState state)
        {
            // Prevent from using money bag after action has been taken to prevent it from being used
            // when Take 5 Coins is available but can't be used this turn
            if (!state.AvailableDice.Any()) return null;
            return state.Player.HasTakenActionThisTurn ? null : new MoneyBag(state.Player, state.AvailableDice.GetLowestDie());
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

        public static IEnumerable<ISpaceActionChoice> VisitBazaar<T>(AiState state) where T : BazaarBase
        {
            var prototype = Bazaar.Create<T>(state.Player);
            var count = prototype.Space.RequiredDice;
            if (!prototype.IsValid() || state.AvailableDice.Count < count) yield break;
            var highValue = state.GetBestDiceValue(count);
            for (var val = 1; val <= highValue; val++)
            {
                var bazaar = Bazaar.Create<T>(state.Player);
                bazaar.Value = val;
                bazaar.Dice = state.AvailableDice.GetLowestDice(count, val);
                yield return bazaar;
            }
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