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
            var space = player.Game.GetActionSpace<TakeContractSpace>();
            var takeContracts = new TakeContracts(player, space);
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
            takeContracts.Die = availableDice.GetLowestDie(takeContracts.Contracts.Max(x => x.Value));
            var cost = player.GetOccupancyCost(space, new[] {takeContracts.Die});
            if (cost.Coin > 0 && !player.CanPay(cost))
                return GenerateResourcesBehavior.GenerateResources(state, cost);
            return takeContracts;
        }

        private static ContractSpace GetEasiestContract(Player player, ResourceBag resources, ActionSpace space, IEnumerable<ContractSpace> contracts, IEnumerable<Die> availableDice)
        {
            return contracts.OrderBy(x => resources.GetShortfall(x.Contract.Cost.Add(player.GetOccupancyCost(space, availableDice.Where(d => d.Value >= x.Value).Take(1)))).Rating).FirstOrDefault();
        }
    }
}
