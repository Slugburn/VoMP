namespace VoMP.Core.CityCards
{
    public static class CityCard
    {
        public static ICityCard[] CreateAll()
        {
            return new ICityCard[]
            {
                new ExchangeCityCard(Cost.Of.Camel(1).And.Silk(1), Reward.Of.Coin(4).And.Vp(2)),
                new ExchangeCityCard(Cost.Of.Pepper(2), Reward.Of.Vp(2)),
                new ExchangeCityCard(Cost.Of.Camel(1).And.Gold(1), Reward.Of.Coin(6).And.Vp(2)),
                new ExchangeCityCard(Cost.Of.Camel(1).And.Gold(1), Reward.Of.Vp(4)),
                new ExchangeCityCard(Cost.Of.Camel(2), Reward.Of.Move(1)),
                new ExchangeCityCard(Cost.Of.Camel(1).And.Pepper(1), Reward.Of.Coin(2).And.Vp(2)),
                new ExchangeCityCard(Cost.Of.Camel(1).And.Gold(1), Reward.Of.Coin(9)),
                new ExchangeCityCard(Cost.Of.Camel(2).And.Silk(1), Reward.Of.Vp(4)),
                new ExchangeCityCard(Cost.Of.Camel(1).And.Pepper(1), Reward.Of.Coin(7)),
                new ExchangeCityCard(Cost.Of.Camel(2), Reward.Of.Coin(2).And.Vp(1)),
                new ExchangeCityCard(Cost.Of.Coin(2), Reward.Of.Good(1)),
                new ExchangeCityCard(Cost.Of.Good(2), Reward.Of.Move(1)),
                new ExchangeCityCard(Cost.Of.Camel(1).And.Silk(1), Reward.Of.Coin(8)),
                new ExchangeCityCard(Cost.Of.Camel(3).And.Pepper(1), Reward.Of.Vp(4)),
                new ExchangeCityCard(Cost.Of.Silk(2), Reward.Of.Vp(3)),
                new ExchangeCityCard(Cost.Of.Camel(1), Reward.Of.Vp(1)),
                new ExchangeCityCard(Cost.Of.Camel(3), Reward.Of.Gold(1).And.Silk(1).And.Pepper(1)),
                new ExchangeCityCard(Cost.Of.Gold(2), Reward.Of.Vp(4)),
                new ExchangeCityCard(Cost.Of.Coin(1), Reward.Of.TradingPostBonus(1)),
                new OptionCityCard(new[] {new ExchangeCityCard(Cost.Of.Camel(1), Reward.Of.Coin(3)), new ExchangeCityCard(Cost.Of.Coin(1), Reward.Of.Camel(1))}),
                OptionCityCard.CreateReversible(Cost.Of.Vp(1), Reward.Of.Coin(3)),
                OptionCityCard.CreateReversible(Cost.Of.Good(1), Reward.Of.Camel(2)),
                new LimitedCityCard(Reward.Of.Coin(2), LimitType.CompletedContract),
                new LimitedCityCard(Reward.Of.Vp(1), LimitType.CompletedContract),
                new LimitedCityCard(Reward.Of.Coin(2), LimitType.TradingPost),
                new LimitedCityCard(Reward.Of.Vp(1), LimitType.TradingPost),
                new DieRangeCityCard(1, 5, Reward.Of.Gold(1), 6, 6, Reward.Of.Gold(3)),
                new DieRangeCityCard(1, 3, Reward.Of.Silk(1), 4, 6, Reward.Of.Silk(3)),
                new DieRangeCityCard(1, 1, Reward.Of.Pepper(1), 2, 6, Reward.Of.Pepper(3)),
                new DieRangeCityCard(1, 4, Reward.Of.TradingPostBonus(1), 5, 6, Reward.Of.TradingPostBonus(2)),
                new DieValueCityCard(Reward.Of.Coin(2))
            };
        }
    }
}