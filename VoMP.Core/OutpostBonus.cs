namespace VoMP.Core
{
    public class OutpostBonus
    {
        public Reward Reward { get; }

        public OutpostBonus(Reward reward)
        {
            Reward = reward;
        }

        static OutpostBonus()
        {
            Catalogue = CreateOutpostBonuses();
        }

        public static OutpostBonus[] Catalogue { get; set; }

        private static OutpostBonus[] CreateOutpostBonuses()
        {
            return new[]
            {
                new OutpostBonus(new Reward {Good = 1}),
                new OutpostBonus(new Reward {Camel = 3}),
                new OutpostBonus(new Reward {Contract = 3}),
                new OutpostBonus(new Reward {Die = 1}),
                new OutpostBonus(new Reward {Silk = 2}),
                new OutpostBonus(new Reward {Move = 1}),
                new OutpostBonus(new Reward {Coin = 5}),
                new OutpostBonus(new Reward {Coin = 3}),
                new OutpostBonus(new Reward {Gold = 2}),
                new OutpostBonus(new Reward {Camel = 2}),
            };
        }

        public override string ToString()
        {
            return Reward.ToString();
        }
    }
}
