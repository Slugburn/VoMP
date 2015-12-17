using System.Collections.Generic;

namespace VoMP.Core
{
    public class Reward
    {
        public int Camel { get; set; }
        public int Coin { get; set; }
        public int Move { get; set; }
        public int Contract { get; set; }
        public int Good { get; set; }
        public int Die { get; set; }
        public int Vp { get; set; }
        public int Gold { get; set; }
        public int Silk { get; set; }
        public int Pepper { get; set; }
        public int CityBonus { get; set; }

        public Cost ToCost()
        {
            return new Cost
            {
                Camel = Camel,
                Coin = Coin,
                Gold = Gold,
                Good = Good,
                Pepper = Pepper,
                Silk = Silk,
                Vp = Vp
            };
        }

        public override string ToString()
        {
            return string.Join(",", GetDescriptions());
        }

        private IEnumerable<string> GetDescriptions()
        {
            if (Camel > 0)
                yield return Camel == 1 ? "Camel" : $"{Camel} Camels";
            if (Coin > 0)
                yield return Coin == 1 ? "Coin" : $"{Coin} Coins";
            if (Move > 0)
                yield return Move == 1 ? "Move" : $"{Move} Moves";
            if (Contract > 0)
                yield return Contract == 1 ? "Contract" : $"{Contract} Contracts";
            if (Good > 0)
                yield return Good == 1 ? "Good" : $"{Good} Goods";
            if (Die > 0)
                yield return Die == 1 ? "Black Die" : $"{Die} Black Dice";
            if (Vp > 0)
                yield return $"{Vp} VP";
            if (Gold > 0)
                yield return Gold == 1 ? "Gold" : $"{Gold} Gold";
            if (Silk > 0)
                yield return Silk == 1 ? "Silk" : $"{Silk} Silk";
            if (Pepper > 0)
                yield return Pepper == 1 ? "Pepper" : $"{Pepper} Pepper";
            if (CityBonus > 0)
                yield return CityBonus == 1 ? "City Bonus" : $"{CityBonus} City Bonuses";
        }

        public Reward Multiply(int factor)
        {
            return new Reward
            {
                Camel = Camel*factor,
                CityBonus = CityBonus*factor,
                Coin = Coin*factor,
                Contract = Contract*factor,
                Die = Die*factor,
                Gold = Gold*factor,
                Good = Good*factor,
                Move = Move*factor,
                Pepper = Pepper*factor,
                Silk = Silk*factor,
                Vp = Vp*factor
            };
        }
    }
}