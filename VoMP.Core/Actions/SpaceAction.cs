using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Extensions;

namespace VoMP.Core.Actions
{
    public abstract class SpaceAction : Action
    {
        protected SpaceAction(string description, int requiredDice) : base(description)
        {
            RequiredDice = requiredDice;
        }

        public int RequiredDice { get; }

        public List<Die> Dice { get; } = new List<Die>();

        public virtual bool IsOccupied => Dice.Any();

        public Cost OccupancyCost(IEnumerable<Die> dice)
        {
            return new Cost {Coin = IsOccupied ? dice.MinValue() : 0};
        }

        public override bool IsValid(Player player)
        {
            // A player can only take one space action per turn
            if (player.HasTakenActionThisTurn) return false;
            // A player can only play colored dice once per round in a space
            var playerColorAlreadyPlayedHere = Dice.Any(x => x.Color == player.Color);
            var availableNonColorDice = player.AvailableDice.Count(x => x.Color != player.Color);
            if (playerColorAlreadyPlayedHere && availableNonColorDice < RequiredDice)
                return false;
            // The player needs to have enough dice left
            return player.AvailableDice.Count >= RequiredDice;
        }
    }
}