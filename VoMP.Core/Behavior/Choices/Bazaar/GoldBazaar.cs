using System.ComponentModel;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    internal class GoldBazaar : BazaarBase
    {
        public GoldBazaar(Player player) : base(player, player.Game.GetActionSpace<GoldBazaarSpace>())
        {
        }

        protected override Reward GetReward(int value)
        {
            switch (value)
            {
                case 1:
                    return new Reward {Gold = 1};
                case 2:
                    return new Reward {Gold = 2};
                case 3:
                    return new Reward {Gold = 2, Camel = 2};
                case 4:
                    return new Reward {Gold = 3};
                case 5:
                    return new Reward {Gold = 3, Move = 1};
                case 6:
                    return new Reward {Gold = 4};
                default:
                    throw new InvalidEnumArgumentException(nameof(value));
            }
        }
    }
}