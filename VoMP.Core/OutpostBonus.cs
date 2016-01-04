namespace VoMP.Core
{
    public class OutpostBonus
    {
        public OutpostBonus(Reward reward)
        {
            Reward = reward;
        }

        public Reward Reward { get; }

        public static OutpostBonus[] CreateAll()
        {
            return new[]
            {
                new OutpostBonus(Core.Reward.Of.Good(1)),
                new OutpostBonus(Core.Reward.Of.Camel(3)),
                new OutpostBonus(Core.Reward.Of.Camel(2)),
                new OutpostBonus(Core.Reward.Of.Contract(1)),
                new OutpostBonus(Core.Reward.Of.Die(1)),
                new OutpostBonus(Core.Reward.Of.Silk(2)),
                new OutpostBonus(Core.Reward.Of.Move(1)),
                new OutpostBonus(Core.Reward.Of.Coin(5)),
                new OutpostBonus(Core.Reward.Of.Coin(3)),
                new OutpostBonus(Core.Reward.Of.Gold(2)),
            };
        }

        public override string ToString()
        {
            return Reward.ToString();
        }
    }
}