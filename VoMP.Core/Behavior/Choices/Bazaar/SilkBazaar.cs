using System.ComponentModel;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    internal class SilkBazaar : BazaarBase
    {
        public SilkBazaar(Player player) : base(player, player.Game.GetActionSpace<SilkBazaarSpace>())
        {
        }

        public override Reward GetReward()
        {
            switch (Value)
            {
                case 1:
                    return new Reward { Silk = 1 };
                case 2:
                    return new Reward { Silk = 2 };
                case 3:
                    return new Reward { Silk = 2, Camel = 1 };
                case 4:
                    return new Reward { Silk = 3 };
                case 5:
                    return new Reward { Silk = 3, Camel = 1, Coin = 1 };
                case 6:
                    return new Reward { Silk = 4 };
                default:
                    throw new InvalidEnumArgumentException(nameof(Value));
            }
        }
    }
}