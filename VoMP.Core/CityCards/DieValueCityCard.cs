using System.Diagnostics;

namespace VoMP.Core.CityCards
{
    internal class DieValueCityCard : ICityCard
    {
        private readonly Reward _reward;

        public DieValueCityCard(Reward reward)
        {
            _reward = reward;
        }

        public override string ToString()
        {
            return $"X }} {_reward}";
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
            return _reward.Multiply(dieValue);
        }
    }
}