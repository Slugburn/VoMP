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

            player.Debug($"reserves resources needed to complete {targetContract}");
            state.ReserveResources(targetContract.Cost);
            var generateResources = GenerateResourcesBehavior.GenerateResources(state, targetContract.Cost);
            if (generateResources != null) return generateResources;
            state.Unreserve(targetContract.Cost);
            return null;
        }
    }
}
