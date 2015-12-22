using System;
using System.Diagnostics;
using System.Linq;

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
            return  $"{range1}:{_reward1}|{range2}:{_reward2}";
        }

        public int OptimumValue(Player player)
        {
            return _min2;
        }

        public Cost GetCost(int dieValue)
        {
            return Cost.None;
        }

        public Reward GetReward(Player player, int dieValue)
        {
            if (_min1 <= dieValue && dieValue <= _max1)
                return _reward1;
            if (_min2 <= dieValue && dieValue <= _max2)
                return _reward2;
            throw new ArgumentOutOfRangeException(nameof(dieValue));
        }

        public bool CanGenerate(Player player, ResourceType resourceType)
        {
            if (_reward1.Gold > 0 &&resourceType == ResourceType.Gold) return true;
            if (_reward1.Silk > 0 && resourceType == ResourceType.Silk) return true;
            if (_reward1.Pepper > 0 && resourceType == ResourceType.Pepper) return true;
            if (_reward1.TradingPostBonus == 0) return false;
            
            // Check to see if the player has any trading post bonuses that meet the requirements
            return player.HasTradingPostBonusFor(resourceType);
        }
    }
}
