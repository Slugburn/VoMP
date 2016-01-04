using System.ComponentModel;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    internal class GoldBazaar : BazaarBase
    {
        public GoldBazaar(Player player) : base(player, player.Game.GetActionSpace<GoldBazaarSpace>())
        {
        }

        protected override Reward RewardFor(int value)
        {
            switch (value)
            {
                case 1:
                    return Reward.Of.Gold(1);
                case 2:
                    return Reward.Of.Gold(2);
                case 3:
                    return Reward.Of.Gold(3).And.Camel(2);
                case 4:
                    return Reward.Of.Gold(3);
                case 5:
                    return Reward.Of.Gold(3).And.Move(1);
                case 6:
                    return Reward.Of.Gold(4);
                default:
                    throw new InvalidEnumArgumentException(nameof(value));
            }
        }
    }
}