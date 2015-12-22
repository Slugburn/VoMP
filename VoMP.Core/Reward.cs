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

        public int OtherCityBonus { get; set; }

        public int TradingPostBonus { get; set; }

        public ResourceBag GetResources()
        {
            return new ResourceBag
            {
                Camel = Camel,
                Coin = Coin,
                Gold = Gold,
                Pepper = Pepper,
                Silk = Silk,
                Vp = Vp
            };
        }

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
                yield return Camel == 1 ? "1 Camel" : $"{Camel} Camels";
            if (Coin > 0)
                yield return Coin == 1 ? "1 Coin" : $"{Coin} Coins";
            if (Move > 0)
                yield return Move == 1 ? "1 Move" : $"{Move} Moves";
            if (Contract > 0)
                yield return Contract == 1 ? "Contract" : $"{Contract} Contracts";
            if (Good > 0)
                yield return Good == 1 ? "1 Good" : $"{Good} Goods";
            if (Die > 0)
                yield return Die == 1 ? "Black Die" : $"{Die} Black Dice";
            if (Vp > 0)
                yield return $"{Vp} VP";
            if (Gold > 0)
                yield return $"{Gold} Gold";
            if (Silk > 0)
                yield return $"{Silk} Silk";
            if (Pepper > 0)
                yield return $"{Pepper} Pepper";
            if (OtherCityBonus > 0)
                yield return OtherCityBonus == 1 ? "1 Other City Bonus" : $"{OtherCityBonus} City Bonuses";
            if (TradingPostBonus > 0)
                yield return TradingPostBonus == 1 ? "1 Trading Post Bonus" : $"{TradingPostBonus} Trading Post Bonuses";
        }

        public Reward Multiply(int factor)
        {
            return new Reward
            {
                Camel = Camel*factor,
                Coin = Coin*factor,
                Contract = Contract*factor,
                Die = Die*factor,
                Gold = Gold*factor,
                Good = Good*factor,
                Move = Move*factor,
                Pepper = Pepper*factor,
                Silk = Silk*factor,
                Vp = Vp*factor,
                OtherCityBonus = OtherCityBonus * factor,
                TradingPostBonus = TradingPostBonus * factor
            };
        }

        public bool CanReward(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Camel:
                    return Camel + OtherCityBonus > 0;
                    case ResourceType.Coin:
                    return Coin + OtherCityBonus > 0;
                    case ResourceType.Gold:
                    return Gold + Good + OtherCityBonus > 0;
                    case ResourceType.Silk:
                    return Silk + Good + OtherCityBonus > 0;
                case ResourceType.Pepper:
                    return Pepper + Good + OtherCityBonus > 0;
                case ResourceType.Vp:
                    return Vp + OtherCityBonus > 0;
                default:
                    return false;
            }
        }
    }
}