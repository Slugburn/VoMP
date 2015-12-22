using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core
{
    public class Cost
    {
        public int Camel { get; set; }
        public int Gold { get; set; }
        public int Silk { get; set; }
        public int Pepper { get; set; }
        public int Coin { get; set; }
        public int Good { get; set; }
        public int Vp { get; set; }
        public static Cost None { get; } = new Cost();

        public int Rating => Camel + Coin + Good + Pepper + Silk * 2 + Gold * 3;

        public Reward ToReward()
        {
            return new Reward {Camel = Camel, Gold = Gold, Silk = Silk, Pepper = Pepper, Coin = Coin, Good = Good, Vp = Vp};
        }

        public override string ToString()
        {
            var descriptions = GetDescriptions().ToList();
            return  descriptions.Any() ? string.Join(",", descriptions) : "nothing";
        }

        private IEnumerable<string> GetDescriptions()
        {
            if (Camel > 0)
                yield return Camel == 1 ? "1 Camel" : $"{Camel} Camels";
            if (Gold > 0)
                yield return $"{Gold} Gold";
            if (Silk > 0)
                yield return $"{Silk} Silk";
            if (Pepper > 0)
                yield return $"{Pepper} Pepper";
            if (Coin > 0)
                yield return Coin == 1 ? "1 Coin" : $"{Coin} Coins";
            if (Good > 0)
                yield return Good == 1 ? "1 Good" : $"{Good} Goods";
            if (Vp > 0)
                yield return $"{Vp} VP";
        }

        public Cost Multiply(int factor)
        {
            return new Cost
            {
                Camel = Camel*factor,
                Coin = Coin*factor,
                Gold = Gold*factor,
                Good = Good*factor,
                Pepper = Pepper*factor,
                Silk = Silk*factor,
                Vp = Vp*factor
            };
        }

        public Cost Add(Cost add)
        {
            return new Cost
            {
                Camel = Camel + add.Camel,
                Coin = Coin + add.Coin,
                Gold = Gold + add.Gold,
                Silk = Silk + add.Silk,
                Pepper = Pepper + add.Pepper,
                Good = Good + add.Good,
                Vp = Vp + add.Vp
            };
        }

        public Cost AllowingFor(Cost add)
        {
            return new Cost
            {
                Camel = Camel > 0 ? Camel + add.Camel : 0,
                Coin = Coin > 0 ? Coin + add.Coin : 0,
                Gold = Gold > 0 ? Gold + add.Gold + add.Good : 0,
                Silk = Silk > 0 ? Silk + add.Silk + add.Good : 0,
                Pepper = Pepper > 0 ? Pepper + add.Pepper + add.Good : 0,
                Good = Good,
                Vp = Vp + add.Vp
            };
        }
    }
}