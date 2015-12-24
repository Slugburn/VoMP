using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.CityCards;
using VoMP.Core.Extensions;

namespace VoMP.Core.Actions
{
    public abstract class ActionSpace : Action
    {
        protected ActionSpace(string description, int requiredDice) : base(description)
        {
            RequiredDice = requiredDice;
        }

        public int RequiredDice { get; }

        protected List<Die> Dice { get; } = new List<Die>();

        public virtual bool IsOccupied => Dice.Any();

        public Cost OccupancyCost(IEnumerable<Die> dice)
        {
            return new Cost {Coin = IsOccupied ? dice.MinValue() : 0};
        }

        public int DiceCount => Dice.Count;

        public void PlayDice(IEnumerable<Die> dice)
        {
            var list = dice as IList<Die> ?? dice.ToList();
            if (list.Any(d => d == null))
                throw new InvalidOperationException("May not add null die");
            Dice.AddRange(list);
        }

        public void ClearDice()
        {
            Dice.Clear();
        }

        public bool ColorAlreadyPlayed(Color color) => Dice.Any(x => x.Color == color);
    }
}