using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    internal class CamelBazaar : BazaarBase
    {
        public CamelBazaar(Player player) : base(player, player.Game.GetActionSpace<CamelBazaarSpace>())
        {
        }

        protected override Reward GetReward(int value)
        {
            return new Reward {Camel = value};
        }
    }
}