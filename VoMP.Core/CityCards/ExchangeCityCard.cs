namespace VoMP.Core.CityCards
{
    public class ExchangeCityCard : ICityCard
    {
        public Cost Cost { get; }
        public Reward Reward { get; }

        public ExchangeCityCard(Cost cost, Reward reward)
        {
            Cost = cost;
            Reward = reward;
        }

        public override string ToString()
        {
            return $"{Cost}->{Reward}";
        }

        public int OptimumValue(Player player)
        {
            for (var value = 6; value > 0; value--)
                if (player.CanPay(GetCost(value)))
                    return value;
            return 0;
        }

        public Cost GetCost(int dieValue)
        {
            return Cost.Multiply(dieValue);
        }

        public Reward GetReward(Player player, int dieValue)
        {
            return Reward.Multiply(dieValue);
        }

        public bool CanGenerate(Player player, ResourceType resourceType)
        {
            return Reward.TradingPostBonus > 0 
                ? player.HasTradingPostBonusFor(resourceType) : 
                Reward.CanReward(resourceType);
        }
    }
}