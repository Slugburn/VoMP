using System;
using System.Collections.Generic;
using VoMP.Core.Actions;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior.Choices
{
    public class Travel : ISpaceActionChoice
    {
        private readonly Dictionary<int, int> _travelCost = new Dictionary<int, int>()
        {
            {1, 3},
            {2, 7},
            {3, 12},
            {4, 12},
            {5, 18},
            {6, 18}
        };

        private readonly Player _player;
        public ActionSpace Space { get; }
        public List<Die> Dice { get; set; }
        public List<Route> Path { get; set; }

        public Travel(Player player)
        {
            _player = player;
            Space = player.Game.GetActionSpace<TravelSpace>();
        }

        public void Execute()
        {
            if (Dice == null || Path == null)
                throw new InvalidOperationException();
            if (Dice.Count < 2)
                throw new InvalidOperationException();
            if (Dice.MinValue() < Path.Count)
                throw new InvalidOperationException();
            _player.PlayDice(Dice, Space);
            var cost = GetTravelCost(Path);
            _player.PayCost(cost, $"travel {Path.Count} spaces");
            _player.Move(Path);
            _player.HasTakenActionThisTurn = true;
            _player.Game.StartPlayer = _player;
        }

        public Cost GetTravelCost(List<Route> path)
        {
            var moveRequired = path.Count;
            return new Cost {Coin = _travelCost[moveRequired]};
        }

        public bool IsValid()
        {
            return _player.CanPlayInActionSpace(Space);
        }

        public Cost GetCost()
        {
            if (Dice == null || Path == null)
                throw new InvalidOperationException();
            return GetTravelCost(Path).Add(_player.GetOccupancyCost(Space, Dice));
        }

        public Reward GetReward()
        {
            return new Reward {Move = Path.Count};
        }
    }
}