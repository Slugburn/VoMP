using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoMP.Core.Behavior.Choices;

namespace VoMP.Core.Behavior
{
    public class CompleteContractBehavior
    {
        public static IActionChoice CompleteContract(AiState state, Func<Contract, bool> condition, Cost adjunctCost)
        {
            var player = state.Player;
            adjunctCost = adjunctCost ?? Cost.None;
            var contracts = player.Contracts.Where(condition).ToList();
            if (!contracts.Any()) return null;
            var completableContract = contracts.OrderByDescending(c => c.Reward.Vp).FirstOrDefault(c => player.Resources.CanPay(c.Cost.Add(adjunctCost)));
            if (completableContract != null)
                return new CompleteContract(player, completableContract);
            // choose the easiest to complete contract to get resources for
            var targetContract = contracts.OrderBy(c => player.Resources.GetShortfall(c.Cost).Rating).First();
            return GenerateResourcesBehavior.GenerateResources(state, targetContract.Cost.Add(adjunctCost));
        }
    }
}
