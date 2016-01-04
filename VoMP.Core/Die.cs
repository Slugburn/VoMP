using System;

namespace VoMP.Core
{
    public class Die
    {
        private static readonly Random Random = new Random();
        private int? _value;

        private Die(Color color)
        {
            Color = color;
        }

        public Color Color { get; set; }

        public int Value
        {
            get
            {
                if (_value == null)
                    throw new UnassignedDieException();
                return _value.Value;
            }
            private set { _value = value; }
        }

        public int SortOrder => _value ?? int.MaxValue;

        public bool HasValue => _value.HasValue;

        public static Die Create(Color color, int? value = null)
        {
            return new Die(color) {_value = value};
        }

        private static int RandomValue()
        {
            return Random.Next(1, 7);
        }

        public void Roll()
        {
            Value = RandomValue();
        }

        public override string ToString()
        {
            var faceValue = _value == null ? "X" : Value.ToString();
            return $"[{Color} {faceValue}]";
        }

        public void Adjust(int direction)
        {
            Value += direction;
        }

        public void Assign(int value)
        {
            _value = value;
        }
    }
}