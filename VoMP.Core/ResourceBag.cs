using System;
using System.Collections.Generic;
using static System.Math;

namespace VoMP.Core
{
    public class ResourceBag
    {
        public int Camel { get; set; }
        public int Coin { get; set; }
        public int Gold { get; set; }
        public int Silk { get; set; }
        public int Pepper { get; set; }
        public int Vp { get; set; }
        public bool CanPay(Cost cost)
        {
            if (cost.Camel > Camel) return false;
            if (cost.Coin > Coin) return false;
            if (cost.Gold > Gold) return false;
            if (cost.Silk > Silk) return false;
            if (cost.Pepper > Pepper) return false;
            if (cost.Good + cost.Gold + cost.Silk + cost.Pepper > Gold + Silk + Pepper) return false;
            if (cost.Vp > Vp) return false;
            return true;
        }

        public ResourceBag Add(ResourceBag add)
        {
            return new ResourceBag
            {
                Camel = Camel + add.Camel,
                Coin = Coin + add.Coin,
                Gold = Gold + add.Gold,
                Silk = Silk + add.Silk,
                Pepper = Pepper + add.Pepper,
                Vp = Vp + add.Vp
            };
        }

        public ResourceBag Subtract(Cost cost)
        {
            var shortfall = GetShortfall(cost);
            if (shortfall.Rating > 0)
                    throw new ApplicationException($"Resources are short by: {shortfall}");
            var resourceBag = new ResourceBag
            {
                Camel = Camel - cost.Camel,
                Coin = Coin - cost.Coin,
                Gold = Gold - cost.Gold,
                Silk = Silk - cost.Silk,
                Pepper = Pepper - cost.Pepper,
                Vp = Vp - cost.Vp
            };
            return resourceBag;
        }

        public Cost GetShortfall(Cost cost)
        {
            var availableGoods = Gold + Silk + Pepper;
            var remainingGoods = Max(availableGoods - cost.Gold + cost.Silk + cost.Pepper, 0);
            return new Cost
            {
                Camel = Abs(Min(Camel - cost.Camel, 0)),
                Coin = Abs(Min(Coin - cost.Coin, 0)),
                Gold = Abs(Min(Gold - cost.Gold, 0)),
                Silk = Abs(Min(Silk - cost.Silk, 0)),
                Pepper = Abs(Min(Pepper - cost.Pepper, 0)),
                Good = Abs(Min(remainingGoods - cost.Good, 0))
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
            if (Gold > 0)
                yield return $"{Gold} Gold";
            if (Silk > 0)
                yield return $"{Silk} Silk";
            if (Pepper > 0)
                yield return $"{Pepper} Pepper";
            if (Vp > 0)
                yield return $"{Vp} VP";
        }

        public Cost ActualizeGoodCost(Cost cost)
        {
            var add = Cost.None;
            add.Pepper = Math.Min(cost.Good, Pepper - cost.Pepper);
            cost.Good -= add.Pepper;
            add.Silk = Math.Min(cost.Good, Silk - cost.Silk);
            cost.Good -= add.Silk;
            add.Gold = Math.Min(cost.Good, Gold - cost.Gold);
            cost.Good -= add.Gold;
            if (cost.Good != 0)
                throw new Exception("Good cost should be 0");
            return cost.Add(add);
        }
    }
}

