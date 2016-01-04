using System.Collections.Generic;

namespace VoMP.Core.Actions
{
    public class MoneyBagSpace : ActionSpace
    {
        public MoneyBagSpace() : base("Money Bag",1)
        {
        }

        // Never considered occupied
        public override bool IsOccupied => false;

        // Player can play as many dice of his color here as necessary
        public override bool ColorAlreadyPlayed(Color color) => false;
    }
}