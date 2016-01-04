using System.ComponentModel;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    internal class SilkBazaar : BazaarBase
    {
        public SilkBazaar(Player player) : base(player, player.Game.GetActionSpace<SilkBazaarSpace>())
        {
        }

        protected override Reward RewardFor(int value)
        {
            switch (value)
            {
                case 1:
                    return Reward.Of.Silk(1);
                case 2:
                    return Reward.Of.Silk(2);
                case 3:
                    return Reward.Of.Silk(2).And.Camel(1);
                case 4:
                    return Reward.Of.Silk(3);
                case 5:
                    return Reward.Of.Silk(3).And.Camel(1).And.Coin(1);
                case 6:
                    return Reward.Of.Silk(4);
                default:
                    throw new InvalidEnumArgumentException(nameof(value));
            }
        }
    }
}