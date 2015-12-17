namespace VoMP.Core.CityCards
{
    internal class ReversibleCityCard : ICityCard
    {
        private readonly Cost _cost;
        private readonly Reward _reward;

        public ReversibleCityCard(Cost cost, Reward reward)
        {
            _cost = cost;
            _reward = reward;
        }

        public int MaxValue(Player player, bool reversed)
        {
            for (var value = 6; value > 0; value--)
                if (player.CanPay(GetCost(value, reversed)))
                    return value;
            return 0;
        }

        public bool IsReversible { get; } = true;

        public Cost GetCost(int dieValue, bool reversed)
        {
            return reversed
                ? _reward.ToCost().Multiply(dieValue)
                : _cost.Multiply(dieValue);
        }

        public Reward GetReward(int dieValue, bool reversed)
        {
            return reversed
                ? _cost.ToReward().Multiply(dieValue)
                : _reward.Multiply(dieValue);
        }

        public override string ToString()
        {
            return $"{_cost} ↔ {_reward}";
        }
    }
}