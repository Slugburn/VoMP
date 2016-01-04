using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core
{
    public class Reward : Reward.IRewardBuilder
    {
        private Reward()
        {
        }

        public int Camel { get; private set; }
        public int Coin { get; private set; }
        public int Gold { get; private set; }
        public int Silk { get; private set; }
        public int Pepper { get; private set; }
        public int Good { get; private set; }
        public int UniqueGood { get; private set; }

        public int Move { get; private set; }
        public int Contract { get; private set; }
        public int Die { get; private set; }
        public int Vp { get; private set; }
        public int OtherCityBonus { get; private set; }
        public int TradingPostBonus { get; private set; }

        public int Rating => Camel + Coin + Move*3 + Gold*3 + Silk*2 + Pepper + Good*3 + UniqueGood*2 + Vp + Die*3 + Move*4 + OtherCityBonus*3;
        public static IRewardBuilder Of => new Reward();
        public IRewardBuilder And => Add(new Reward());
        public static Reward None { get; } = new Reward();

        Reward IRewardBuilder.Camel(int count)
        {
            Camel = count;
            return this;
        }

        Reward IRewardBuilder.Coin(int count)
        {
            Coin = count;
            return this;
        }

        Reward IRewardBuilder.Gold(int count)
        {
            Gold = count;
            return this;
        }

        Reward IRewardBuilder.Silk(int count)
        {
            Silk = count;
            return this;
        }

        Reward IRewardBuilder.Pepper(int count)
        {
            Pepper = count;
            return this;
        }

        Reward IRewardBuilder.Vp(int count)
        {
            Vp = count;
            return this;
        }

        Reward IRewardBuilder.TradingPostBonus(int count)
        {
            TradingPostBonus = count;
            return this;
        }

        Reward IRewardBuilder.OtherCityBonus(int count)
        {
            OtherCityBonus = count;
            return this;
        }

        Reward IRewardBuilder.Good(int count)
        {
            Good = count;
            return this;
        }

        Reward IRewardBuilder.UniqueGood(int count)
        {
            UniqueGood = count;
            return this;
        }

        Reward IRewardBuilder.Move(int count)
        {
            Move = count;
            return this;
        }

        Reward IRewardBuilder.Contract(int count)
        {
            Contract = count;
            return this;
        }

        Reward IRewardBuilder.Die(int count)
        {
            Die = count;
            return this;
        }

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
            return Cost.Of
                .Camel(Camel)
                .And.Coin(Coin)
                .And.Gold(Gold)
                .And.Silk(Silk)
                .And.Pepper(Pepper)
                .And.Vp(Vp)
                .And.Good(Good + UniqueGood)
                .And.Move(Move);
        }

        public override string ToString()
        {
            var descriptions = GetDescriptions().ToList();
            if (descriptions.Count == 1)
                return descriptions.Single();
            return $"({string.Join(",", descriptions)})";
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
            if (UniqueGood > 0)
                yield return $"{UniqueGood} unique Goods";
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
                UniqueGood = UniqueGood*factor,
                Move = Move*factor,
                Pepper = Pepper*factor,
                Silk = Silk*factor,
                Vp = Vp*factor,
                OtherCityBonus = OtherCityBonus*factor,
                TradingPostBonus = TradingPostBonus*factor
            };
        }

        public Reward Add(Reward reward)
        {
            return new Reward
            {
                Camel = Camel + reward.Camel,
                Coin = Coin + reward.Coin,
                Contract = Contract + reward.Contract,
                Die = Die + reward.Die,
                Gold = Gold + reward.Gold,
                Good = Good + reward.Good,
                UniqueGood = UniqueGood + reward.UniqueGood,
                Move = Move + reward.Move,
                Pepper = Pepper + reward.Pepper,
                Silk = Silk + reward.Silk,
                Vp = Vp + reward.Vp,
                OtherCityBonus = OtherCityBonus + reward.OtherCityBonus,
                TradingPostBonus = TradingPostBonus + reward.TradingPostBonus
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
                    return Gold + Good + UniqueGood + OtherCityBonus > 0;
                case ResourceType.Silk:
                    return Silk + Good + UniqueGood + OtherCityBonus > 0;
                case ResourceType.Pepper:
                    return Pepper + Good + UniqueGood + OtherCityBonus > 0;
                case ResourceType.Vp:
                    return Vp + OtherCityBonus > 0;
                case ResourceType.Move:
                    return Move > 0;
                default:
                    return false;
            }
        }

        public Reward GetConcreteRewards()
        {
            return new Reward
            {
                Camel = Camel,
                Coin = Coin,
                Contract = Contract,
                Die = Die,
                Gold = Gold,
                Move = Move,
                Pepper = Pepper,
                Silk = Silk,
                Vp = Vp
            };
        }

        public interface IRewardBuilder
        {
            Reward Camel(int count);
            Reward Coin(int count);
            Reward Gold(int count);
            Reward Silk(int count);
            Reward Pepper(int count);
            Reward Vp(int count);
            Reward TradingPostBonus(int count);
            Reward OtherCityBonus(int count);
            Reward Good(int count);
            Reward UniqueGood(int count);
            Reward Move(int count);
            Reward Contract(int count);
            Reward Die(int count);
        }
    }
}