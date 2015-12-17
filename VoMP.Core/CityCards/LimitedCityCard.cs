using System;
using System.ComponentModel;
using System.Diagnostics;

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

        public bool IsReversible { get; } = false;

        public int MaxValue(Player player, bool reversed)
        {
            Debug.Assert(!reversed);
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