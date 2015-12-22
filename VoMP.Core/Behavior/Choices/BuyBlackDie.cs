﻿namespace VoMP.Core.Behavior.Choices
{
    public class BuyBlackDie : IActionChoice
    {
        private readonly Player _player;

        public BuyBlackDie(Player player)
        {
            _player = player;
        }

        public Cost Cost { get; } = new Cost {Camel = 3};

        public void Execute()
        {
            _player.PayCost(Cost, "take a Black Die");
            _player.GainBlackDie();
            _player.HasBoughtBlackDieThisTurn = true;
        }

        public bool IsValid()
        {
            return _player.Game.BlackDice > 0 && !_player.HasBoughtBlackDieThisTurn;
        }
    }
}