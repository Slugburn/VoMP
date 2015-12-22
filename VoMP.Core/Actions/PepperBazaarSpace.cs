using System.ComponentModel;

namespace VoMP.Core.Actions
{
    public class PepperBazaarSpace : GrandBazaarSpace
    {
        public PepperBazaarSpace() : base("Grand Bazaar (Pepper)", 1)
        {
        }

        public override Reward GetReward(int value)
        {
            switch (value)
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
                    throw new InvalidEnumArgumentException(nameof(value));
            }
        }
    }
}