using System.Collections.Generic;

namespace VoMP.Core.Actions
{
    public abstract class GrandBazaarSpace : SpaceAction
    {
        protected GrandBazaarSpace(string description, int requiredDice) : base(description, requiredDice)
        {
        }

        public void Execute(Player player, int value, List<Die> dice)
        {
            player.PlayDice(dice, this);
            player.GainReward(GetReward(value), Description);
            player.HasTakenActionThisTurn = true;
        }

        public abstract Reward GetReward(int value);
    }
}
