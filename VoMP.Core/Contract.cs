namespace VoMP.Core
{
    public class Contract
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
                new Contract(new Cost {Camel = 2, Silk = 1, Pepper = 1}, new Reward {Contract = 1, Vp = 5}),
                new Contract(new Cost {Camel = 2, Gold = 1, Pepper = 1}, new Reward {Die = 1, Vp = 4}),
                new Contract(new Cost {Camel = 1, Pepper = 3}, new Reward {Move = 1, Vp = 1}),
                new Contract(new Cost {Camel = 2, Gold = 1, Silk = 1}, new Reward {Good = 2, Vp = 4}),
                new Contract(new Cost {Camel = 1, Gold = 1}, new Reward {Camel = 3, Vp = 2}),
                new Contract(new Cost {Camel = 1, Silk = 2}, new Reward {Coin = 5, Vp = 3})
            };
        }

        public static Contract[] CreateContracts()
        {
            return new[]
            {
                new Contract(new Cost {Camel = 2, Silk = 2, Pepper = 1}, new Reward {Coin = 3, Vp = 4}),
                new Contract(new Cost {Camel = 2, Gold = 2, Pepper = 3}, new Reward {Camel = 5, Vp = 7}),
                new Contract(new Cost {Camel = 2, Gold = 3, Silk = 2}, new Reward {Move = 1, Vp = 9}),
                new Contract(new Cost {Camel = 1, Gold = 4}, new Reward {Camel = 3, Vp = 7}),
                new Contract(new Cost {Camel = 2, Gold = 3, Pepper = 2}, new Reward {Coin = 7, Vp = 8}),
                new Contract(new Cost {Camel = 3, Gold = 1, Silk = 1, Pepper = 1}, new Reward {Contract = 1, Vp = 6}),
                new Contract(new Cost {Camel = 2, Silk = 3, Pepper = 2}, new Reward {Good = 1, Die = 1, Vp = 5}),
                new Contract(new Cost {Camel = 2, Gold = 1, Pepper = 2}, new Reward {Contract = 1, Vp = 4}),
                new Contract(new Cost {Camel = 1, Gold = 3}, new Reward {Good = 1, Vp = 6}),
                new Contract(new Cost {Camel = 3, Gold = 1, Silk = 1, Pepper = 1}, new Reward {Die = 1, Vp = 5}),
                new Contract(new Cost {Camel = 2, Silk = 2, Pepper = 2}, new Reward {Die = 1, Vp = 5}),
                new Contract(new Cost {Camel = 2, Gold = 2, Silk = 2}, new Reward {Contract = 1, Vp = 7}),
                new Contract(new Cost {Camel = 1, Pepper = 4}, new Reward {Contract = 1, Vp = 3}),
                new Contract(new Cost {Camel = 3, Gold = 1, Silk = 1, Pepper = 1}, new Reward {Good = 1, Vp = 6}),
                new Contract(new Cost {Camel = 1, Silk = 3}, new Reward {Good = 1, Vp = 3}),
                new Contract(new Cost {Camel = 2, Gold = 2, Silk = 2}, new Reward {Move = 1, Vp = 7}),
                new Contract(new Cost {Camel = 1, Gold = 4}, new Reward {Move = 1, Vp = 7}),
                new Contract(new Cost {Camel = 2, Gold = 2, Silk = 1}, new Reward {Coin = 4, Vp = 6}),
                new Contract(new Cost {Camel = 2, Gold = 2, Silk = 2}, new Reward {Camel = 4, Vp = 6}),
                new Contract(new Cost {Camel = 2, Gold = 2, Silk = 3}, new Reward {Contract = 1, Vp = 8}),
                new Contract(new Cost {Camel = 3, Gold = 1, Silk = 2, Pepper = 1},
                    new Reward {Camel = 3, Contract = 1, Vp = 5}),
                new Contract(new Cost {Camel = 3, Gold = 1, Silk = 2, Pepper = 1},
                    new Reward {Camel = 3, Coin = 4, Vp = 5}),
                new Contract(new Cost {Camel = 2, Silk = 2, Pepper = 3}, new Reward {Move = 1, Vp = 5}),
                new Contract(new Cost {Camel = 1, Pepper = 3}, new Reward {Die = 1, Vp = 2}),
                new Contract(new Cost {Camel = 2, Silk = 1, Pepper = 2}, new Reward {Good = 2, Vp = 3}),
                new Contract(new Cost {Camel = 3, Gold = 1, Silk = 1, Pepper = 1}, new Reward {Good = 2, Vp = 5}),
                new Contract(new Cost {Camel = 2, Gold = 2, Pepper = 1}, new Reward {Die = 1, Vp = 5}),
                new Contract(new Cost {Camel = 2, Gold = 2, Silk = 1}, new Reward {Die = 1, Vp = 6}),
                new Contract(new Cost {Camel = 2, Gold = 2, Pepper = 3}, new Reward {Coin = 7, Vp = 6}),
                new Contract(new Cost {Camel = 3, Gold = 2, Silk = 1, Pepper = 1}, new Reward {Coin = 5, Vp = 7}),
                new Contract(new Cost {Camel = 2, Gold = 1, Pepper = 2}, new Reward {Move = 1, Vp = 4}),
                new Contract(new Cost {Camel = 3, Gold = 1, Silk = 1, Pepper = 1}, new Reward {Move = 1, Vp = 5}),
                new Contract(new Cost {Camel = 2, Silk = 3, Pepper = 2}, new Reward {Coin = 6, Vp = 5}),
                new Contract(new Cost {Camel = 2, Gold = 3, Silk = 2}, new Reward {Coin = 7, Vp = 9}),
                new Contract(new Cost {Camel = 2, Gold = 1, Silk = 2}, new Reward {Camel = 4, Vp = 4}),
                new Contract(new Cost {Camel = 2, Silk = 1, Pepper = 2}, new Reward {Coin = 4, Vp = 3}),
                new Contract(new Cost {Camel = 2, Gold = 3, Pepper = 2}, new Reward {Good = 2, Vp = 9}),
                new Contract(new Cost {Camel = 2, Gold = 2, Silk = 3}, new Reward {Camel = 4, Vp = 8})
            };
        }
    }
}