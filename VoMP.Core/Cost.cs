using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace VoMP.Core
{
    public class Cost : Cost.IBuilder
    {
        private Cost()
        {
        }

        public static Cost None { get; } = new Cost();

        public int Camel { get; private set; }
        public int Gold { get; private set; }
        public int Silk { get; private set; }
        public int Pepper { get; private set; }
        public int Coin { get; private set; }
        public int Good { get; private set; }
        public int Vp { get; private set; }
        public int Move { get; private set; }

        public int Rating => Camel + Coin + Good + Pepper + Silk*2 + Gold*3 + Vp*2 + Move*4;

        public static IBuilder Of => new Cost();
        public IBuilder And => Add(new Cost());

        Cost IBuilder.Camel(int count)
        {
            Camel = count;
            return this;
        }

        Cost IBuilder.Coin(int count)
        {
            Coin = count;
            return this;
        }

        Cost IBuilder.Gold(int count)
        {
            Gold = count;
            return this;
        }

        Cost IBuilder.Silk(int count)
        {
            Silk = count;
            return this;
        }

        Cost IBuilder.Pepper(int count)
        {
            Pepper = count;
            return this;
        }

        Cost IBuilder.Vp(int count)
        {
            Vp = count;
            return this;
        }

        Cost IBuilder.Good(int count)
        {
            Good = count;
            return this;
        }

        Cost IBuilder.Move(int count)
        {
            Move = count;
            return this;
        }

        public Reward ToReward()
        {
            return Reward.Of.Camel(Camel).And.Coin(Coin).And.Gold(Gold).And.Silk(Silk).And.Pepper(Pepper).And.Good(Good).And.Vp(Vp).And.Move(Move);
        }

        public override string ToString()
        {
            var descriptions = GetDescriptions().ToList();
            switch (descriptions.Count)
            {
                case 0:
                    return "nothing";
                case 1:
                    return descriptions.Single();
                default:
                    return $"({string.Join(",", descriptions)})";
            }
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
            if (Move > 0)
                yield return $"{Move} Move";
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
                Vp = Vp*factor,
                Move = Move*factor
            };
        }

        public Cost Add(Cost add)
        {
            var gold = Gold + add.Gold;
            var silk = Silk + add.Silk;
            var pepper = Pepper + add.Pepper;
            var good = Good + add.Good;
            return new Cost
            {
                Camel = Max(Camel + add.Camel, 0),
                Coin =  Max(Coin + add.Coin,0),
                Gold = Max(gold, 0),
                Silk = Max(silk, 0),
                Pepper = Max(pepper, 0),
                Good = Max(good, 0),
                Vp = Max(Vp + add.Vp, 0),
                Move = Max(Move + add.Move, 0)
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
                Vp = Vp + add.Vp,
                Move = Move > 0 ? Move + add.Move : 0
            };
        }

        public Cost Subtract(Cost cost)
        {
            return Add(cost.Multiply(-1));
        }

        public Cost Subtract(Reward reward)
        {
            return Subtract(reward.ToCost());
        }

        public interface IBuilder
        {
            Cost Camel(int count);
            Cost Coin(int count);
            Cost Gold(int count);
            Cost Silk(int count);
            Cost Pepper(int count);
            Cost Vp(int count);
            Cost Good(int count);
            Cost Move(int count);
        }

        public Cost GetConcreteCosts()
        {
            return new Cost
            {
                Camel = Camel,
                Coin = Coin,
                Gold = Gold,
                Pepper = Pepper,
                Silk = Silk,
                Vp = Vp,
                Move = Move
            };
        }
    }

}