using System;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bonus
{
    public class MoneyBag : IActionChoice
    {
        private readonly Player _player;
        private readonly MoneyBagSpace _space;
        private readonly Die _die;

        public MoneyBag(Player player, MoneyBagSpace space, Die die)
        {
            if (die == null)
                throw new InvalidOperationException();
            _player = player;
            _space = space;
            _die = die;
        }

        public void Execute()
        {
            _player.TakeDie(_die);
            _space.Dice.Add(_die);
            _player.GainReward(new Reward { Coin = 3 }, $"placing {_die} in Money Bag");
        }

        public bool IsValid()
        {
            return _die != null;
        }
    }
}