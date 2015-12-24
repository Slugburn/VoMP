﻿using System;
using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;

namespace VoMP.Core.Behavior.Choices
{
    public class TakeContracts : ISpaceActionChoice
    {
        public List<ContractSpace> Contracts { get; } = new List<ContractSpace>();
        public Die Die { get; set; }
        private readonly Player _player;
        private readonly TakeContractSpace _space;

        public TakeContracts(Player player, TakeContractSpace space)
        {
            _player = player;
            _space = space;
        }

        public void Execute()
        {
            _player.PlayDice(new[] { Die }, _space);
            foreach (var space in Contracts)
            {
                _player.Output($"takes contract for {space.Contract} at position {space.Value}");
                _player.Game.AvailableContracts.Remove(space.Contract);
                _player.GainContract(space.Contract);
                if (space.Value == 5)
                    _player.GainReward(_player.ChooseCamelOrCoin(1), "Take Contracts [5]");
                if (space.Value == 6)
                    _player.GainReward(_player.ChooseCamelOrCoin(2), "Take Contracts [6]");
            }
            _player.HasTakenActionThisTurn = true;
        }

        public bool IsValid()
        {
            return _player.Game.AvailableContracts.Any() && _player.CanPlayInActionSpace(_space);
        }

        public Cost GetCost()
        {
            return _player.GetOccupancyCost(_space, new[] {Die});
        }

        public Reward GetReward()
        {
            return new Reward {Contract = Contracts.Count };
        }
    }
}