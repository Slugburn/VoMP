using System.ComponentModel;

namespace VoMP.Core.Actions
{
    public class GoldBazaarSpace : GrandBazaarSpace
    {
        public GoldBazaarSpace() : base("Grand Bazaar (Gold)", 3)
        {
        }

        public override Reward GetReward(int value)
        {
            switch (value)
            {
                case 1:
                    return new Reward { Gold = 1 };
                case 2:
                    return new Reward { Gold = 2 };
                case 3:
                    return new Reward { Gold = 2, Camel = 2 };
                case 4:
                    return new Reward { Gold = 3 };
                case 5:
                    return new Reward { Gold = 3, Move = 1 };
                case 6:
                    return new Reward { Gold = 4 };
                default:
                    throw new InvalidEnumArgumentException(nameof(value));
            }
        }
    }
}
