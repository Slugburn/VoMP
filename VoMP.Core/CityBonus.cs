using System;

namespace VoMP.Core
{
    public class CityBonus
    {
        public CityBonus(Reward reward)
        {
            Reward = reward;
        }

        public Reward Reward { get; set; }

        public static CityBonus[] CreateAll()
        {
            return new []
            {
                new CityBonus(new Reward {Camel = 3}), 
                new CityBonus(new Reward {OtherCityBonus = 1}), 
                new CityBonus(new Reward {Vp = 3}), 
                new CityBonus(new Reward {Good = 2}), 
                new CityBonus(new Reward {Coin = 5}), 
                new CityBonus(new Reward {Camel = 1, Coin = 3}), 
            };

        }

        public override string ToString()
        {
            return $"!<{Reward}>";
        }
    }
}