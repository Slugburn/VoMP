using System;
using System.Diagnostics;

namespace VoMP.Core.CityCards
{
    public class DieRangeCityCard : ICityCard
    {
        private readonly int _min1;
        private readonly int _max1;
        private readonly Reward _reward1;
        private readonly int _min2;
        private readonly int _max2;
        private readonly Reward _reward2;

        public DieRangeCityCard(int min1, int max1, Reward reward1, int min2, int max2, Reward reward2)
        {
            _min1 = min1;
            _max1 = max1;
            _reward1 = reward1;
            _min2 = min2;
            _max2 = max2;
            _reward2 = reward2;
        }

        public override string ToString()
        {
            var range1 = _min1 == _max1 ? _min1.ToString() : $"{_min1}-{_max1}";
            var range2 = _min2 == _max2 ? _min2.ToString() : $"{_min2}-{_max2}";
            return  $"[{range1}] {_reward1}; [{range2}] {_reward2}";
        }

        public bool IsReversible { get; } = false;

        public int MaxValue(Player player, bool reversed)
        {
            Debug.Assert(!reversed);
            return 6;
        }

        public Cost GetCost(int dieValue, bool reversed)
        {
            Debug.Assert(!reversed);
            return Cost.None;
        }

        public Reward GetReward(int dieValue, bool reversed)
        {
            Debug.Assert(!reversed);
            if (_min1 <= dieValue && dieValue <= _max1)
                return _reward1.Multiply(dieValue);
            if (_min2 <= dieValue && dieValue <= _max2)
                return _reward2.Multiply(dieValue);
            throw new ArgumentOutOfRangeException(nameof(dieValue));
        }
    }
}
