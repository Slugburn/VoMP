using System.ComponentModel;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    public class PepperBazaar : BazaarBase
    {
        public PepperBazaar(Player player) : base(player, player.Game.GetActionSpace<PepperBazaarSpace>())
        {
        }

        public override Reward GetReward()
        {
            switch (Value)
            {
                case 1:
                    return new Reward { Pepper = 1 };
                case 2:
                    return new Reward { Pepper = 2 };
                case 3:
                    return new Reward { Pepper = 2, Coin = 1 };
                case 4:
                    return new Reward { Pepper = 3 };
                case 5:
                    return new Reward { Pepper = 3, Coin = 2 };
                case 6:
                    return new Reward { Pepper = 4 };
                default:
                    throw new InvalidEnumArgumentException(nameof(Value));
            }
        }
    }
}