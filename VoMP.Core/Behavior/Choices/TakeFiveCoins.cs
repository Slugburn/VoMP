using System;
using System.Collections.Generic;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices
{
    public class TakeFiveCoins : ISpaceActionChoice
    {
        private readonly Player _player;
        public ActionSpace Space { get; }

        public Die Die { get; set; }

        public TakeFiveCoins(Player player)
        {
            _player = player;
            Space = player.Game.GetActionSpace<TakeFiveCoinsSpace>();
        }

        public void Execute()
        {
            if (Die==null)
                throw new InvalidOperationException("Die has not been set.");
            _player.PlayDice(new[] { Die }, Space);
            _player.GainReward(GetReward(), Space.Description);
            _player.HasTakenActionThisTurn = true;
        }

        public Cost GetCost()
        {

            return _player.GetOccupancyCost(Space, new[] {Die},1);
        }

        public Reward GetReward()
        {
            return new Reward { Coin = 5 };
        }

        public IList<Die> Dice => new[] {Die};
        public int Value => 1;

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(Space);
        }
    }
}
