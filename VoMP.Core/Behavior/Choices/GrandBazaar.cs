using System;
using System.Collections.Generic;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices
{
    public class GrandBazaar : IActionChoice
    {
        private readonly Player _player;
        private readonly GrandBazaarSpace _space;
        private readonly int _value;
        private readonly List<Die> _dice;

        public GrandBazaar(Player player, GrandBazaarSpace space, int value, List<Die> dice)
        {
            if (space==null)
                throw new ArgumentNullException(nameof(space));
            _player = player;
            _space = space;
            _value = value;
            _dice = dice;
        }

        public void Execute()
        {
            _space.Execute(_player, _value, _dice);
        }

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(_space);
        }
    }
}