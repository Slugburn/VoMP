using System;
using System.Collections.Generic;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bonus
{
    public class MoneyBag : ISpaceActionChoice
    {
        private readonly Die _die;
        private readonly Player _player;

        public MoneyBag(Player player, Die die)
        {
            if (die == null)
                throw new InvalidOperationException();
            _player = player;
            Space = player.Game.GetActionSpace<MoneyBagSpace>();
            _die = die;
        }

        public void Execute()
        {
            _player.PlayDice(new[] {_die}, Space);
            _player.GainReward(Reward, $"placing {_die} in Money Bag");
        }

        public bool IsValid()
        {
            return _die != null;
        }

        public Cost Cost => Cost.None;

        public Reward Reward => Reward.Of.Coin(3);

        public IList<Die> Dice => new[] {_die};
        public int Value => 1;
        public ActionSpace Space { get; }
    }
}