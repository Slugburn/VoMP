using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core.Actions
{
    public class TravelSpace : SpaceAction
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

        public TravelSpace() : base("Travel", 2)
        {
        }

        public void Execute(Player player, List<Route> path, List<Die> dice)
        {
            var cost = GetCost(path, dice);
            player.PlayDice(dice, this);

            player.Move(path);
            player.HasTakenActionThisTurn = true;
            player.Game.StartPlayer = player;
        }

        public Cost GetCost(List<Route> path, List<Die> dice)
        {
            var moveRequired = path.Count;
            var cost = path.GetCost();
            cost.Coin += _travelCost[moveRequired];
            return cost;
        }
    }
}
