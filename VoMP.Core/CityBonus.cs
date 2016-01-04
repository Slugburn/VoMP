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
            return new[]
            {
                new CityBonus(Reward.Of.Camel(3)),
                new CityBonus(Reward.Of.OtherCityBonus(1)),
                new CityBonus(Reward.Of.Vp(3)),
                new CityBonus(Reward.Of.UniqueGood(2)),
                new CityBonus(Reward.Of.Coin(5)),
                new CityBonus(Reward.Of.Camel(1).And.Coin(3))
            };
        }

        public override string ToString()
        {
            return $"!<{Reward}>";
        }
    }
}