using System;
using System.Collections.Generic;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices.Bazaar
{
    public abstract class BazaarBase : ISpaceActionChoice
    {
        private readonly Player _player;

        protected BazaarBase(Player player, GrandBazaarSpace space)
        {
            _player = player;
            Space = space;
        }

        public ActionSpace Space { get; set; }

        public int Value { get; set; }
        public IList<Die> Dice { get; set; }

        public void Execute()
        {
            if (Dice == null) throw new InvalidOperationException();
            _player.PlayDice(Dice, Space);
            _player.GainReward(Reward, Space.Description);
            _player.HasTakenActionThisTurn = true;
        }

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(Space);
        }

        public Cost Cost => _player.GetOccupancyCost(Space, Dice, Value);

        public Reward Reward => RewardFor(Value);

        protected abstract Reward RewardFor(int value);
    }
}