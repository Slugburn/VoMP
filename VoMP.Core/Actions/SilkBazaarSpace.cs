using System.ComponentModel;

namespace VoMP.Core.Actions
{
    public class SilkBazaarSpace : GrandBazaarSpace
    {
        public SilkBazaarSpace() : base("Grand Bazaar (Silk)", 2)
        {
        }

        public override Reward GetReward(int value)
        {
            switch (value)
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
                    throw new InvalidEnumArgumentException(nameof(value));
            }
        }
    }
}