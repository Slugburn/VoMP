using System;

namespace VoMP.Core
{
    public class CityBonus
    {
        static CityBonus()
        {
            Catalogue = CreateCityBonuses();
        }

        public CityBonus(Reward reward)
        {
            Reward = reward;
        }

        public Reward Reward { get; set; }
        public static CityBonus[] Catalogue { get; set; }

        private static CityBonus[] CreateCityBonuses()
        {
            return new []
            {
                new CityBonus(new Reward {Camel = 3}), 
                new CityBonus(new Reward {CityBonus = 1}), 
                new CityBonus(new Reward {Vp = 3}), 
                new CityBonus(new Reward {Good = 2}), 
                new CityBonus(new Reward {Coin = 5}), 
                new CityBonus(new Reward {Camel = 1, Coin = 3}), 
            };
        }
    }
}