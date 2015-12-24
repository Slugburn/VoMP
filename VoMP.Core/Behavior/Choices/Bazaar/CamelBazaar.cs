using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    public class CamelBazaar : BazaarBase
    {
        public CamelBazaar(Player player) : base(player, player.Game.GetActionSpace<CamelBazaarSpace>())
        {
        }

        public override Reward GetReward()
        {
            return new Reward {Camel = Value};
        }
    }
}