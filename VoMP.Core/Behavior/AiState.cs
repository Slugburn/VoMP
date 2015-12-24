using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;
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

        public List<Route> BestPath { get; }
        public List<Route> NextMove { get; }

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

        public void ClearResourceReserves()
        {
            ReservedResources = Cost.None;
        }

        public bool PlayerCanPay(Cost cost)
        {
            return Player.CanPay(cost.AllowingFor(ReservedResources));
        }

        public Cost GetShortfall(Cost cost)
        {
            return Player.Resources.GetShortfall(cost.Add(ReservedResources));
        }

        public IActionChoice MakeChoiceWithReservedResources(ReserveResourcesChoiceParam p)
        {
            if (p.Cost != null && p.Cost.Rating > 0)
            {
                Player.Debug($"reserves {p.Cost} needed to {p.Reason}");
                ReservedResources = ReservedResources.Add(p.Cost);
            }
            if (p.Dice != null)
            {
                Player.Debug($"reserves {p.Dice.ToDelimitedString("")} needed to {p.Reason}");
                ReservedDice.AddRange(p.Dice);
            }
            var choice = p.MakeChoice();
            if (choice != null) return choice;
            if (p.Cost != null)
                ReservedResources.Subtract(p.Cost);
            p.Dice?.ForEach(d => ReservedDice.Remove(d));
            return null;
        }
    }
}