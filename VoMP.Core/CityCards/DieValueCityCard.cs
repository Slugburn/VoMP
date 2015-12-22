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
            return $"X->{_reward}";
        }

        public int OptimumValue(Player player)
        {
            return 5;
        }

        public Cost GetCost(int dieValue)
        {
            return Cost.None;
        }

        public Reward GetReward(Player player, int dieValue)
        {
            return _reward.Multiply(dieValue);
        }

        public bool CanGenerate(Player player, ResourceType resourceType)
        {
            return resourceType == ResourceType.Coin;
        }
    }
}