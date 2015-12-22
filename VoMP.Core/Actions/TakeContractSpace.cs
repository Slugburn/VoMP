using System.Linq;

namespace VoMP.Core.Actions
{
    public class TakeContractSpace : SpaceAction
    {
        public TakeContractSpace() : base("Take Contracts", 1)
        {
        }

        public override bool IsValid(Player player)
        {
            return base.IsValid(player)
                // There must be contracts available to take
                && player.Game.AvailableContracts.Any();
        }
    }
}
