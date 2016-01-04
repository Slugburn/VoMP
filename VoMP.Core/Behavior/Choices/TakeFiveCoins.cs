using System;
using System.Collections.Generic;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices
{
    public class TakeFiveCoins : ISpaceActionChoice
    {
        private readonly Player _player;

        public TakeFiveCoins(Player player)
        {
            _player = player;
            Space = player.Game.GetActionSpace<TakeFiveCoinsSpace>();
        }

        public Die Die { get; set; }
        public ActionSpace Space { get; }

        public void Execute()
        {
            if (Die == null)
                throw new InvalidOperationException("Die has not been set.");
            _player.PlayDice(new[] {Die}, Space);
            _player.GainReward(Reward, Space.Description);
            _player.HasTakenActionThisTurn = true;
        }

        public Cost Cost => _player.GetOccupancyCost(Space, new[] {Die}, 1);

        public Reward Reward => Reward.Of.Coin(5);

        public IList<Die> Dice => new[] {Die};
        public int Value => 1;

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(Space);
        }
    }
}