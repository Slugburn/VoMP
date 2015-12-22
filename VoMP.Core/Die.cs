using System;

namespace VoMP.Core
{
    public class Die
    {
        private static readonly Random Random = new Random();

        public Die(Color color)
        {
            Color = color;
            Value = RandomValue();
        }

        public Color Color { get; set; }

        public int Value { get; set; }

        private static int RandomValue()
        {
            return Random.Next(1, 7);
        }

        public void Reroll()
        {
            Value = RandomValue();
        }

        public override string ToString()
        {
            return $"[{Color} {Value}]";
        }

        public void Adjust(int direction)
        {
            Value += direction;
        }
    }
}