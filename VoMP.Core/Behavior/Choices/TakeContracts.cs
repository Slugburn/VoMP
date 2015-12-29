using System;
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
        public ActionSpace Space { get; }

        public TakeContracts(Player player)
        {
            _player = player;
            Space = player.Game.GetActionSpace<TakeContractSpace>();
        }

        public void Execute()
        {
            _player.PlayDice(new[] { Die }, Space);
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
            return _player.Game.AvailableContracts.Any() && _player.CanPlayInActionSpace(Space);
        }

        public Cost GetCost()
        {
            return _player.GetOccupancyCost(Space, Dice, Value);
        }

        public Reward GetReward()
        {
            return new Reward {Contract = Contracts.Count };
        }

        public IList<Die> Dice => new[] {Die};
        public int Value => Contracts.Max(c => c.Value);
    }
}
