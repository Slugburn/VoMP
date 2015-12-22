using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core
{
    public class Route
    {
        public Route(Location start, Location end)
        {
            Start = start;
            End = end;
        }

        public Location Start { get; set; }
        public Location End { get; set; }
        public int Camel { get; set; }
        public int Coin { get; set; }
    }

    public static class RouteExtensions
    {
        public static Cost GetCost(this IEnumerable<Route> path)
        {
            return path.Aggregate(new Cost(), (c, r) =>
            {
                c.Camel += r.Camel;
                c.Coin += r.Coin;
                return c;
            });
        }
    }
}
