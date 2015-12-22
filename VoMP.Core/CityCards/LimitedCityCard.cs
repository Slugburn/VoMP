using System;
using System.ComponentModel;

namespace VoMP.Core.CityCards
{
    public class LimitedCityCard : ICityCard
    {
        private readonly Reward _reward;
        private readonly LimitType _limitType;

        public LimitedCityCard(Reward reward, LimitType limitType)
        {
            _reward = reward;
            _limitType = limitType;
        }

        public override string ToString()
        {
            var description = GetDescription(_limitType);
            return $"{_reward} / {description}";
        }

        private static string GetDescription(LimitType limitType)
        {
            switch (limitType)
            {
                case LimitType.CompletedContract:
                    return "Completed Contract";
                case LimitType.TradingPost:
                    return "Trading Post";
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public int OptimumValue(Player player)
        {
            return GetLimit(player);
        }

        private int GetLimit(Player player)
        {
            switch (_limitType)
            {
                case LimitType.CompletedContract:
                    return Math.Min(player.CompletedContracts.Count, 6);
                case LimitType.TradingPost:
                    return Math.Min(player.TradingPosts.Count, 6);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public Cost GetCost(int dieValue)
        {
            return Cost.None;
        }

        public Reward GetReward(Player player, int dieValue)
        {
            var value = Math.Min(GetLimit(player), dieValue);
            return _reward.Multiply(value);
        }

        public bool CanGenerate(Player player, ResourceType resourceType)
        {
            var meetsLimit = OptimumValue(player) > 0;
            if (!meetsLimit) return false;
            switch (resourceType)
            {
                case ResourceType.Coin:
                    return _reward.Coin > 0;
                case ResourceType.Vp:
                    return _reward.Vp > 0;
                default:
                    return false;
            }
        }
    }
}