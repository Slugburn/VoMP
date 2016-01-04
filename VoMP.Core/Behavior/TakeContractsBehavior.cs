using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    class TakeContractsBehavior
    {
        public static IActionChoice TakeContracts(AiState state, Func<Contract, bool> condition)
        {
            var player = state.Player;
            if (player.Contracts.Count == 2) return null;

            var space = player.Game.GetActionSpace<TakeContractSpace>();
            var takeContracts = new TakeContracts(player);
            if (!takeContracts.IsValid()) return null;

            var availableDice = state.GetDiceAvailableFor(space);
            var maxValue = availableDice.MaxValue();
            var contracts = player.Game.GetContractSpaces().Where(x => condition(x.Contract) && x.Value <= maxValue).ToList();
            if (!contracts.Any()) return null;

            // Get easiest contract to complete
            var first = GetEasiestContract(player, player.Resources, space, contracts, availableDice);
            takeContracts.Contracts.Add(first);
            // Take a second contract?
            if (player.Contracts.Count == 0)
            {
                var resourcesAfterFirst = player.Resources.Add(player.Resources.GetShortfall(first.Contract.Cost).ToReward().GetResources()).Add(first.Contract.Reward.GetResources());
                var second = GetEasiestContract(player, resourcesAfterFirst, space, contracts.Except(new[] {first}), availableDice);
                if (second!=null)
                    takeContracts.Contracts.Add(second);
            }
            var die = availableDice.GetLowestDie(takeContracts.Contracts.Max(x => x.Value));
            takeContracts.Die = die;
            var cost = takeContracts.Cost;
            if (player.CanPay(cost)) return takeContracts;
            // Generate resources needed to take contract
            using (state.ReserveDice(new[] {die}, "Take Contracts"))
                return GenerateResourcesBehavior.GenerateResources(state, cost, "Take Contracts");
        }

        private static ContractSpace GetEasiestContract(Player player, ResourceBag resources, ActionSpace space, IEnumerable<ContractSpace> contracts, IEnumerable<Die> availableDice)
        {
            return contracts.OrderBy(x => resources.GetShortfall(
                GetTotalCost(player, space, availableDice, x))
                .Rating)
                .FirstOrDefault();
        }

        private static Cost GetTotalCost(Player player, ActionSpace takeContractsSpace, IEnumerable<Die> availableDice, ContractSpace x)
        {
            return x.Contract.Cost.Add(player.GetOccupancyCost(takeContractsSpace, availableDice.Where(d => !d.HasValue || d.Value >= x.Value).Take(1), x.Value));
        }
    }
}
