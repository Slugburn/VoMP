using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Extensions;

namespace VoMP.Core.CityCards
{
    internal class OptionCityCard : ICityCard
    {
        private ExchangeCityCard _selectedOption;
        private List<ExchangeCityCard> _options;

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
            _selectedOption = null;
        }

        public void SelectResouce(Player player, ResourceType resourceType)
        {
            _selectedOption = _options.SingleOrDefault(o => o.CanGenerate(player, resourceType));
        }

        public int OptimumValue(Player player)
        {
            return MaxValue(player);
        }

        public Cost GetCost(int dieValue)
        {
            return _selectedOption?.Cost.Multiply(dieValue);
        }

        public Reward GetReward(Player player, int dieValue)
        {
            return _selectedOption?.Reward.Multiply(dieValue);
        }

        public bool CanGenerate(Player player, ResourceType resourceType)
        {
            return _options.Any(o=>o.CanGenerate(player, resourceType));
        }

        private int MaxValue(Player player)
        {
            for (var value = 6; value > 0; value--)
                if (player.CanPay(_selectedOption.Cost.Multiply(value)))
                    return value;
            return 0;
        }

        public override string ToString()
        {
            return _options.ToDelimitedString(" or ");
        }
    }
}