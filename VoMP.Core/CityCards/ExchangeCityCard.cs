using System.Diagnostics;

namespace VoMP.Core.CityCards
{
    public class ExchangeCityCard : ICityCard
    {
        private readonly Cost _cost;
        private readonly Reward _reward;

        public ExchangeCityCard(Cost cost, Reward reward)
        {
            _cost = cost;
            _reward = reward;
        }

        public override string ToString()
        {
            return $"{_cost} }} {_reward}";
        }

        public int MaxValue(Player player, bool reversed)
        {
            Debug.Assert(!reversed);
            for (var value = 6; value > 0; value--)
                if (player.CanPay(GetCost(value, false)))
                    return value;
            return 0;
        }

        public bool IsReversible { get; } = false;

        public Cost GetCost(int dieValue, bool reversed)
        {
            Debug.Assert(!reversed);
            return _cost.Multiply(dieValue);
        }

        public Reward GetReward(int dieValue, bool reversed)
        {
            Debug.Assert(!reversed);
            return _reward.Multiply(dieValue);
        }
    }
}