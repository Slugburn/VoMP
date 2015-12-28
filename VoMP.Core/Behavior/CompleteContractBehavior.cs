using System;
using System.Linq;
using VoMP.Core.Behavior.Choices;
using VoMP.Core.Behavior.Choices.Bonus;

namespace VoMP.Core.Behavior
{
    public class CompleteContractBehavior
    {
        public static IActionChoice CompleteContract(AiState state, Func<Contract, bool> condition)
        {
            var player = state.Player;
            var contracts = player.Contracts.Where(condition).ToList();
            if (!contracts.Any()) return null;
            var completableContract = contracts.OrderByDescending(c => c.Reward.Vp).FirstOrDefault(c => state.PlayerCanPay(c.Cost));
            if (completableContract != null)
                return new CompleteContract(player, completableContract);
            // choose the easiest to complete contract to get resources for
            var targetContract = contracts.OrderBy(c => player.Resources.GetShortfall(c.Cost).Rating).First();

            string reason = $"complete {targetContract}";
            var cost = targetContract.Cost;
            return GenerateResourcesBehavior.GenerateResources(state, cost, reason);
        }
    }
}
