using System.Collections.Generic;
using System.Linq;
using VoMP.Core.Extensions;

namespace VoMP.Core
{
    public class Route
    {
        public Route(Location start, Location end, Cost cost = null)
        {
            Start = start;
            End = end;
            Cost = cost ?? Cost.None;
        }

        public Location Start { get; set; }
        public Location End { get; set; }
        public Cost Cost { get; set; }
    }

    public static class RouteExtensions
    {
        public static Cost GetCost(this IEnumerable<Route> path)
        {
            return path.Select(r => r.Cost).Total();
        }
    }
}
