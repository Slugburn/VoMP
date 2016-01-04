using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Extensions;

namespace VoMP.Core.CityCards
{
    internal class OptionCityCard : ICityCard
    {
        private readonly List<ExchangeCityCard> _options;

        public static OptionCityCard CreateReversible(Cost cost, Reward reward)
        {
            return new OptionCityCard(new[]
            {
                new ExchangeCityCard(cost, reward),
                new ExchangeCityCard(reward.ToCost(), cost.ToReward())
            });
        }

        public OptionCityCard(IEnumerable<ExchangeCityCard> options)
        {
            _options = options.ToList();
        }

        public int OptimumValue(Player player)
        {
            return MaxValue(player);
        }

        public Cost GetCost(int dieValue)
        {
            throw new NotImplementedException();
        }

        public Reward GetReward(Player player, int dieValue)
        {
            throw new NotImplementedException();
        }

        public bool CanGenerate(Player player, ResourceType resourceType)
        {
            return _options.Any(o=>o.CanGenerate(player, resourceType));
        }

        private int MaxValue(Player player)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return _options.ToDelimitedString(" or ");
        }

        public IEnumerable<ExchangeCityCard> GetOptions(Player player, ResourceType resourceType)
        {
            return _options.Where(o => o.CanGenerate(player, resourceType));
        }
        public IEnumerable<ExchangeCityCard> GetOptions(Player player)
        {
            return _options;
        }
    }
}