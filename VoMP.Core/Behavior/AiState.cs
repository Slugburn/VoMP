using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Extensions;

namespace VoMP.Core.Behavior
{
    public class AiState
    {
        public AiState(Player player)
        {
            Player = player;
            BestPath = player.GetBestPath();
            NextMove = BestPath != null ? GetMoves(BestPath, player.TradingPosts).First() : null;
        }

        public Player Player { get; }

        public List<Route> BestPath { get;  }
        public List<Route> NextMove { get;  }

        public Cost ReservedResources { get; private set; } = Cost.None;
        public List<Die> ReservedDice { get; } = new List<Die>();

        public Cost Shortfall { get; set; }

        public List<Die> GetDiceAvailableFor(ActionSpace space) => Player.GetDiceAvailableFor(space).Except(ReservedDice).ToList();

        public List<Die> AvailableDice => Player.AvailableDice.Except(ReservedDice).ToList();


        private static IEnumerable<List<Route>> GetMoves(List<Route> path, ISet<Location> tradingPosts)
        {
            var list = new List<Route>();
            foreach (var route in path)
            {
                list.Add(route);
                if (!route.End.IsTradeLocation() || tradingPosts.Contains(route.End)) continue;
                yield return list;
                list = new List<Route>();
            }
        }

        public void ReserveResources(Cost cost)
        {
            ReservedResources = ReservedResources.Add(cost);
        }

        public void ReserveDice(IEnumerable<Die> dice)
        {
            ReservedDice.AddRange(dice);
        }

        public void ClearResourceReserves()
        {
            ReservedResources = Cost.None;
        }

        public bool PlayerCanPay(Cost cost)
        {
            return Player.CanPay(cost.AllowingFor(ReservedResources));
        }

        public void Reserve(Cost cost, IEnumerable<Die> dice)
        {
            ReserveResources(cost);
            ReserveDice(dice);
        }

        public void Unreserve(Cost cost, IEnumerable<Die> dice = null)
        {
            ReservedResources.Subtract(cost);
            dice?.ForEach(d => ReservedDice.Remove(d));
        }
    }
}