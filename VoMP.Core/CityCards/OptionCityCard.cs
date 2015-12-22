using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Extensions;

namespace VoMP.Core.CityCards
{
    internal class OptionCityCard : ICityCard
    {
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
            Options = options.ToList();
        }

        public List<ExchangeCityCard> Options { get; set; }

        public int OptimumValue(Player player)
        {
            return MaxValue(player, 0);
        }

        public Cost GetCost(int dieValue)
        {
            return GetCost(dieValue, 0);
        }

        public Reward GetReward(Player player, int dieValue)
        {
            return GetReward(dieValue, 0);
        }

        public bool CanGenerate(Player player, ResourceType resourceType)
        {
            return Options.Any(o=>o.CanGenerate(player, resourceType));
        }

        public int MaxValue(Player player, int option)
        {
            for (var value = 6; value > 0; value--)
                if (player.CanPay(GetCost(value, option)))
                    return value;
            return 0;
        }

        public Cost GetCost(int dieValue, int option)
        {
            return Options[option].Cost.Multiply(dieValue);
        }

        public Reward GetReward(int dieValue, int option)
        {
            return Options[option].Reward.Multiply(dieValue);
        }

        public override string ToString()
        {
            return Options.ToDelimitedString(" or ");
        }
    }
}