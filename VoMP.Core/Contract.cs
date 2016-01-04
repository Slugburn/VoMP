namespace VoMP.Core
{
    public class Contract : IExchange
    {
        public Contract(Cost cost, Reward reward)
        {
            Cost = cost;
            Reward = reward;
        }

        public Cost Cost { get; set; }
        public Reward Reward { get; set; }

        public override string ToString()
        {
            return $"{Cost}->{Reward}";
        }

        public static Contract[] CreateStartingContracts()
        {
            return new[]
            {
                new Contract(Cost.Of.Camel(2).And.Silk(1).And.Pepper(1), Reward.Of.Contract(1).And.Vp(5)),
                new Contract(Cost.Of.Camel(2).And.Gold(1).And.Pepper(1), Reward.Of.Die(1).And.Vp(4)),
                new Contract(Cost.Of.Camel(1).And.Pepper(3), Reward.Of.Move(1).And.Vp(1)),
                new Contract(Cost.Of.Camel(2).And.Gold(1).And.Silk(1), Reward.Of.UniqueGood(2).And.Vp(4)),
                new Contract(Cost.Of.Camel(1).And.Gold(1), Reward.Of.Camel(3).And.Vp(2)),
                new Contract(Cost.Of.Camel(1).And.Silk(2), Reward.Of.Coin(5).And.Vp(3))
            };
        }

        public static Contract[] CreateContracts()
        {
            return new[]
            {
                new Contract(Cost.Of.Camel(2).And.Silk(2).And.Pepper(1), Reward.Of.Coin(3).And.Vp(4)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Pepper(3), Reward.Of.Camel(5).And.Vp(7)),
                new Contract(Cost.Of.Camel(2).And.Gold(3).And.Silk(2), Reward.Of.Move(1).And.Vp(9)),
                new Contract(Cost.Of.Camel(1).And.Gold(4), Reward.Of.Camel(3).And.Vp(7)),
                new Contract(Cost.Of.Camel(2).And.Gold(3).And.Pepper(2), Reward.Of.Coin(7).And.Vp(8)),
                new Contract(Cost.Of.Camel(3).And.Gold(1).And.Silk(1).And.Pepper(1), Reward.Of.Contract(1).And.Vp(6)),
                new Contract(Cost.Of.Camel(2).And.Silk(3).And.Pepper(2), Reward.Of.Good(1).And.Die(1).And.Vp(5)),
                new Contract(Cost.Of.Camel(2).And.Gold(1).And.Pepper(2), Reward.Of.Contract(1).And.Vp(4)),
                new Contract(Cost.Of.Camel(1).And.Gold(3), Reward.Of.Good(1).And.Vp(6)),
                new Contract(Cost.Of.Camel(3).And.Gold(1).And.Silk(1).And.Pepper(1), Reward.Of.Die(1).And.Vp(5)),
                new Contract(Cost.Of.Camel(2).And.Silk(2).And.Pepper(2), Reward.Of.Die(1).And.Vp(5)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Silk(2), Reward.Of.Contract(1).And.Vp(7)),
                new Contract(Cost.Of.Camel(1).And.Pepper(4), Reward.Of.Contract(1).And.Vp(3)),
                new Contract(Cost.Of.Camel(3).And.Gold(1).And.Silk(1).And.Pepper(1), Reward.Of.Good(1).And.Vp(6)),
                new Contract(Cost.Of.Camel(1).And.Silk(3), Reward.Of.Good(1).And.Vp(3)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Silk(2), Reward.Of.Move(1).And.Vp(7)),
                new Contract(Cost.Of.Camel(1).And.Gold(4), Reward.Of.Move(1).And.Vp(7)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Silk(1), Reward.Of.Coin(4).And.Vp(6)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Silk(2), Reward.Of.Camel(4).And.Vp(6)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Silk(3), Reward.Of.Contract(1).And.Vp(8)),
                new Contract(Cost.Of.Camel(3).And.Gold(1).And.Silk(2).And.Pepper(1), Reward.Of.Camel(3).And.Contract(1).And.Vp(5)),
                new Contract(Cost.Of.Camel(3).And.Gold(1).And.Silk(2).And.Pepper(1), Reward.Of.Camel(3).And.Coin(4).And.Vp(5)),
                new Contract(Cost.Of.Camel(2).And.Silk(2).And.Pepper(3), Reward.Of.Move(1).And.Vp(5)),
                new Contract(Cost.Of.Camel(1).And.Pepper(3), Reward.Of.Die(1).And.Vp(2)),
                new Contract(Cost.Of.Camel(2).And.Silk(1).And.Pepper(2), Reward.Of.UniqueGood(2).And.Vp(3)),
                new Contract(Cost.Of.Camel(3).And.Gold(1).And.Silk(1).And.Pepper(1), Reward.Of.UniqueGood(2).And.Vp(5)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Pepper(1), Reward.Of.Die(1).And.Vp(5)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Silk(1), Reward.Of.Die(1).And.Vp(6)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Pepper(3), Reward.Of.Coin(7).And.Vp(6)),
                new Contract(Cost.Of.Camel(3).And.Gold(2).And.Silk(1).And.Pepper(1), Reward.Of.Coin(5).And.Vp(7)),
                new Contract(Cost.Of.Camel(2).And.Gold(1).And.Pepper(2), Reward.Of.Move(1).And.Vp(4)),
                new Contract(Cost.Of.Camel(3).And.Gold(1).And.Silk(1).And.Pepper(1), Reward.Of.Move(1).And.Vp(5)),
                new Contract(Cost.Of.Camel(2).And.Silk(3).And.Pepper(2), Reward.Of.Coin(6).And.Vp(5)),
                new Contract(Cost.Of.Camel(2).And.Gold(3).And.Silk(2), Reward.Of.Coin(7).And.Vp(9)),
                new Contract(Cost.Of.Camel(2).And.Gold(1).And.Silk(2), Reward.Of.Camel(4).And.Vp(4)),
                new Contract(Cost.Of.Camel(2).And.Silk(1).And.Pepper(2), Reward.Of.Coin(4).And.Vp(3)),
                new Contract(Cost.Of.Camel(2).And.Gold(3).And.Pepper(2), Reward.Of.UniqueGood(2).And.Vp(9)),
                new Contract(Cost.Of.Camel(2).And.Gold(2).And.Silk(3), Reward.Of.Camel(4).And.Vp(8))
            };
        }
    }
}