namespace VoMP.Core.CityCards
{
    public static class CityCard
    {
        public static ICityCard[] CreateAll()
        {
            return new ICityCard[]
            {
                new ExchangeCityCard(new Cost {Camel = 1, Silk = 1}, new Reward {Coin = 4, Vp = 2}),
                new ExchangeCityCard(new Cost {Pepper = 2}, new Reward {Vp = 2}),
                new ExchangeCityCard(new Cost {Camel = 1, Gold = 1}, new Reward {Coin = 6, Vp = 2}),
                new ExchangeCityCard(new Cost {Camel = 1, Gold = 1}, new Reward {Vp = 4}),
                new ExchangeCityCard(new Cost {Camel = 2}, new Reward {Move = 1}),
                new ExchangeCityCard(new Cost {Camel = 1, Pepper = 1}, new Reward {Coin = 2, Vp = 2}),
                new ExchangeCityCard(new Cost {Camel = 1, Gold = 1}, new Reward {Coin = 9}),
                new ExchangeCityCard(new Cost {Camel = 2, Silk = 1}, new Reward {Vp = 4}),
                new ExchangeCityCard(new Cost {Camel = 1, Pepper = 1}, new Reward {Coin = 7}),
                new ExchangeCityCard(new Cost {Camel = 2}, new Reward {Coin = 2, Vp = 1}),
                new ExchangeCityCard(new Cost {Coin = 2}, new Reward {Good = 1}),
                new ExchangeCityCard(new Cost {Good = 2}, new Reward {Move = 1}),
                new ExchangeCityCard(new Cost {Camel = 1, Silk = 1}, new Reward {Coin = 8}),
                new ExchangeCityCard(new Cost {Camel = 3, Pepper = 1}, new Reward {Vp = 4}),
                new ExchangeCityCard(new Cost {Silk = 2}, new Reward {Vp = 3}),
                new ExchangeCityCard(new Cost {Camel = 1}, new Reward {Vp = 1}),
                new ExchangeCityCard(new Cost {Camel = 3}, new Reward {Gold = 1, Silk = 1, Pepper = 1}),
                new ExchangeCityCard(new Cost {Gold = 2}, new Reward {Vp = 4}),
                new ExchangeCityCard(new Cost {Coin = 1}, new Reward {TradingPostBonus = 1}),
                new OptionCityCard(new [] {new ExchangeCityCard(new Cost {Camel = 1}, new Reward {Coin = 3}), new ExchangeCityCard(new Cost {Coin=1}, new Reward {Camel = 1} )}),
                OptionCityCard.CreateReversible(new Cost {Vp = 1}, new Reward {Coin = 3}),
                OptionCityCard.CreateReversible(new Cost {Good = 1}, new Reward {Camel = 2}),
                new LimitedCityCard(new Reward {Coin = 2}, LimitType.CompletedContract),
                new LimitedCityCard(new Reward {Vp = 1}, LimitType.CompletedContract),
                new LimitedCityCard(new Reward {Coin = 2}, LimitType.TradingPost),
                new LimitedCityCard(new Reward {Vp = 1}, LimitType.TradingPost),
                new DieRangeCityCard(1,5,new Reward{Gold = 1}, 6,6,new Reward {Gold=3} ),
                new DieRangeCityCard(1,3,new Reward {Silk = 1},4,6,new Reward {Silk=3}), 
                new DieRangeCityCard(1,1,new Reward {Pepper = 1},2,6,new Reward {Pepper = 3}), 
                new DieRangeCityCard(1,4,new Reward {TradingPostBonus = 1},5,6,new Reward {TradingPostBonus = 2}), 
                new DieValueCityCard(new Reward {Coin=2 })
            };
        }
    }
}