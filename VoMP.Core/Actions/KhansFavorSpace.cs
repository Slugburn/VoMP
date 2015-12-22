using System;
using System.Linq;
using VoMP.Core.Extensions;
using static VoMP.Core.ResourceType;

namespace VoMP.Core.Actions
{
    public class KhansFavorSpace : SpaceAction
    {
        public KhansFavorSpace() : base("The Khan's Favor", 1)
        {
        }

        public override bool IsValid(Player player)
        {
            return base.IsValid(player) 
                // Must be less than 4 dice played in space
                && Dice.Count < 4
                // Player must have die with value >= max value already played
                && (player.AvailableDice.Any(d=>d.Value >= MinimumValue));
        }

        internal void Execute(Player player, ResourceType resourceType, Die die)
        {
            player.PlayDice(new [] {die}, this);
            var reward = new Reward {Camel = 2, Gold = resourceType == Gold ? 1: 0, Silk = resourceType==Silk ? 1: 0, Pepper = resourceType==Pepper ? 1 : 0};
            player.GainReward(reward , Description );
            player.HasTakenActionThisTurn = true;
        }

        // Is never considered occupied
        public override bool IsOccupied => false;

        public int MinimumValue => Dice.Any() ? Dice.MaxValue() : 1;
    }
}
