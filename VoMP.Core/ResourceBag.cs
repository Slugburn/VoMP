using System;
using System.Collections.Generic;
using static System.Math;

namespace VoMP.Core
{
    public class ResourceBag : IEquatable<ResourceBag>
    {
        private int _camel;
        private int _coin;
        private int _gold;
        private int _silk;
        private int _pepper;

        public int Camel
        {
            get { return _camel; }
            set
            {
                if (value < 0 ) throw new ArgumentException(nameof(Camel));
                _camel = value;
            }
        }

        public int Coin
        {
            get { return _coin; }
            set
            {
                if (value < 0) throw new ArgumentException(nameof(Coin));
                _coin = value;
            }
        }

        public int Gold
        {
            get { return _gold; }
            set
            {
                if (value < 0) throw new ArgumentException(nameof(Gold));
                _gold = value;
            }
        }

        public int Silk
        {
            get { return _silk; }
            set
            {
                if (value < 0) throw new ArgumentException(nameof(Silk));
                _silk = value;
            }
        }

        public int Pepper
        {
            get { return _pepper; }
            set
            {
                if (value < 0) throw new ArgumentException(nameof(Pepper));
                _pepper = value;
            }
        }

        public int Vp { get; set; }

        public bool CanPay(Cost cost)
        {
            if (cost.Camel > Camel) return false;
            if (cost.Coin > Coin) return false;
            if (cost.Gold > Gold) return false;
            if (cost.Silk > Silk) return false;
            if (cost.Pepper > Pepper) return false;
            if (cost.Good + cost.Gold + cost.Silk + cost.Pepper > Gold + Silk + Pepper) return false;
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
            var camel = Abs(Min(Camel - cost.Camel, 0));
            var coin = Abs(Min(Coin - cost.Coin, 0));
            var gold = Abs(Min(Gold - cost.Gold, 0));
            var silk = Abs(Min(Silk - cost.Silk, 0));
            var pepper = Abs(Min(Pepper - cost.Pepper, 0));
            var good = Abs(Min(remainingGoods - cost.Good, 0));
            return Cost.Of
                .Camel(camel)
                .And.Coin(coin)
                .And.Gold(gold)
                .And.Silk(silk)
                .And.Pepper(pepper)
                .And.Good(good);
        }

        public bool Equals(ResourceBag other) => Camel == other.Camel && Coin == other.Coin && Gold == other.Gold && Silk == other.Silk && Pepper == other.Pepper && Vp == other.Vp;

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
            var good = cost.Good;
            var pepper = Min(good, Pepper - cost.Pepper);
            good -= pepper;
            var silk = Min(good, Silk - cost.Silk);
            good -= silk;
            var gold = Min(good, Gold - cost.Gold);
            good -= gold;
            if (good != 0)
                throw new Exception("Good cost should be 0");
            return Cost.Of.Gold(gold).And.Silk(silk).And.Pepper(pepper);
        }
    }
}

