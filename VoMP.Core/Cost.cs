using System.Collections.Generic;

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

        public Reward ToReward()
        {
            return new Reward {Camel = Camel, Gold = Gold, Silk = Silk, Pepper = Pepper, Coin = Coin, Good = Good, Vp = Vp};
        }

        public override string ToString()
        {
            return string.Join(",", GetDescriptions());
        }

        private IEnumerable<string> GetDescriptions()
        {
            if (Camel > 0)
                yield return Camel == 1 ? "Camel" : $"{Camel} Camels";
            if (Gold > 0)
                yield return Gold == 1 ? "Gold" : $"{Gold} Gold";
            if (Silk > 0)
                yield return Silk == 1 ? "Silk" : $"{Silk} Silk";
            if (Pepper > 0)
                yield return Pepper == 1 ? "Pepper" : $"{Pepper} Pepper";
            if (Coin > 0)
                yield return Coin == 1 ? "Coin" : $"{Coin} Coins";
            if (Good > 0)
                yield return Good == 1 ? "Good" : $"{Good} Goods";
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
    }
}