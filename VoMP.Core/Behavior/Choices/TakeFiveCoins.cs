using System;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices
{
    public class TakeFiveCoins : IActionChoice
    {
        private readonly Player _player;
        private readonly TakeFiveCoinsSpace _space;
        private readonly Die _die;

        public TakeFiveCoins(Player player, TakeFiveCoinsSpace space, Die die)
        {
            _player = player;
            _space = space;
            _die = die;
        }

        public void Execute()
        {
            _player.PlayDice(new[] { _die }, _space);
            _player.GainReward(new Reward { Coin = 5 }, _space.Description);
            _player.HasTakenActionThisTurn = true;
        }

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(_space);
        }
    }
}
