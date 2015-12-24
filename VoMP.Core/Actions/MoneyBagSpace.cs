using System.Collections.Generic;

namespace VoMP.Core.Actions
{
    public class MoneyBagSpace : Action
    {
        public MoneyBagSpace() : base("Money Bag")
        {
        }

        public List<Die> Dice { get; } = new List<Die>();
    }
}