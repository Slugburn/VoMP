using System;
using System.Linq;
using VoMP.Core.Extensions;

namespace VoMP.Core.Actions
{
    public class KhansFavorSpace : ActionSpace
    {
        public KhansFavorSpace() : base("The Khan's Favor", 1)
        {
        }

        // Is never considered occupied
        public override bool IsOccupied => false;

        public int MinimumValue => Dice.Any() ? Dice.MaxValue() : 1;
    }
}
