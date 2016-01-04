using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    public class CamelBazaar : BazaarBase
    {
        public CamelBazaar(Player player) : base(player, player.Game.GetActionSpace<CamelBazaarSpace>())
        {
        }

        protected override Reward RewardFor(int value)
        {
            return Reward.Of.Camel(Value);
        }
    }
}