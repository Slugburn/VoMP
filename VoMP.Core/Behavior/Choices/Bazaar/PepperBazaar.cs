using System.ComponentModel;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    public class PepperBazaar : BazaarBase
    {
        public PepperBazaar(Player player) : base(player, player.Game.GetActionSpace<PepperBazaarSpace>())
        {
        }

        protected override Reward RewardFor(int value)
        {
            switch (value)
            {
                case 1:
                    return Reward.Of.Pepper(1);
                case 2:
                    return Reward.Of.Pepper(2);
                case 3:
                    return Reward.Of.Pepper(2).And.Coin(1);
                case 4:
                    return Reward.Of.Pepper(3);
                case 5:
                    return Reward.Of.Pepper(3).And.Coin(2);
                case 6:
                    return Reward.Of.Pepper(4);
                default:
                    throw new InvalidEnumArgumentException(nameof(value));
            }
        }

        public override string ToString()
        {
            return $"Pepper Bazaar ({Value})";
        }
    }
}