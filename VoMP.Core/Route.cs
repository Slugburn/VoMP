using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static VoMP.Core.Location;

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

    public static class RouteMap
    {
        private static Route[] _routes;
        private static volatile ILookup<Location, Route> _map;

        static RouteMap()
        {
            var routes = new List<Route>
            {
                new Route(Venezia, OasisA),
                new Route(OasisA, Moscow),
                new Route(Moscow, Anxi) {Camel = 3},
                new Route(Anxi, Karakorum) {Camel = 2},
                new Route(Karakorum, OasisB),
                new Route(OasisB, OasisC) {Coin = 5},
                new Route(OasisC, Beijing),
                new Route(OasisB, OasisD),
                new Route(OasisD, Beijing) {Camel = 3},
                new Route(Venezia, OasisE) {Camel = 3},
                new Route(OasisE, Samarcanda),
                new Route(Samarcanda, OasisF) {Camel = 2},
                new Route(OasisF, Kashgar) {Camel = 4},
                new Route(Kashgar, LanZhou) {Camel = 2},
                new Route(LanZhou, OasisD),
                new Route(OasisE, OasisG),
                new Route(Venezia, Alexandria),
                new Route(Alexandria, OasisG) {Coin = 7},
                new Route(OasisG, Ormuz) {Camel = 3},
                new Route(Ormuz, Karachi) {Camel = 3},
                new Route(Karachi, OasisH),
                new Route(OasisH, Xian),
                new Route(Xian, Beijing),
                new Route(Alexandria, OasisI),
                new Route(OasisI, Adana) {Camel = 2},
                new Route(Adana, Kochi) {Coin = 15},
                new Route(Kochi, OasisH) {Camel = 4},
                new Route(Kochi, Sumatra) {Coin = 10},
                new Route(Sumatra, OasisJ) {Camel = 4},
                new Route(OasisJ, Beijing),
            };
            // Add reverse direction
            var reverseDirection = routes.Select(r=>new Route(r.End,r.Start) {Camel = r.Camel, Coin = r.Coin}).ToList();
            routes.AddRange(reverseDirection);

            _map = routes.ToLookup(r => r.Start);
        }

        public static List<Route> ShortestPath(Location start, Location end)
        {
            var locations = Enum.GetValues(typeof (Location)).Cast<Location>().ToList();
            var dist = new Dictionary<Location, int>();
            var prev = new Dictionary<Location,Location>();
            var Q = new HashSet<Location>(locations);
            foreach (var loc in locations)
                dist[loc] = int.MaxValue;
            dist[start] = 0;
            while (Q.Any())
            {
                var u = Q.OrderBy(x => dist[x]).First();
                if (u == end)
                {
                    var path = new List<Route>();
                    while (u != start)
                    {
                        var p = prev[u];
                        path.Add(_map[p].Single(x => x.End == u));
                        u = p;
                    }
                    path.Reverse();
                    return path;
                }
                Q.Remove(u);
                foreach (var v in _map[u].Select(x=>x.End))
                {
                    var alt = dist[u] + 1;
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }
            throw new Exception("No path found");
        }

        public static List<Route> ShortestPath(IList<Location> locations)
        {
            var list = new List<Route>();
            for (var i = 0; i < locations.Count() - 1; i++)
            {
                list.AddRange(ShortestPath(locations[i], locations[i + 1]));
            }
            return list;
        }

        public static IEnumerable<List<Route>> GetPaths(Location start, Location end)
        {
            return GetPaths(start, end, ImmutableHashSet.Create<Location>());
        }

        private static IEnumerable<List<Route>> GetPaths(Location start, Location end, ImmutableHashSet<Location> visited)
        {
            var outgoing = _map[start];
            foreach (var route in outgoing.Where(route => !visited.Contains(route.End)))
            {
                if (route.End == end)
                    yield return new List<Route> {route};
                foreach (var subPath in GetPaths(route.End, end, visited.Union(new[] {start})))
                    yield return (new[] {route}.Concat(subPath)).ToList();
            }
        }

        public static List<Route> BestPath(Location start, params Location[] targets)
        {
            var permutations = GeneratePermutations(targets).ToList();
            return permutations
                .Select(ts => ShortestPath(new[] {start}.Union(ts).ToList()))
                .OrderBy(p => p.Count)
                .First();

            return targets.ToList().SelectMany(t => GetPaths(start, t))
                .Where(path => path.Select(route => route.End).Intersect(targets).SequenceEqual(targets))
                .OrderBy(path => path.Count)
                .First();
        }

        public static IEnumerable<IEnumerable<T>> GeneratePermutations<T>(IEnumerable<T> enumerable)
        {
            var list = enumerable as IList<T> ?? enumerable.ToList();
            if (list.Count == 1) return new[] { list };
            return list.SelectMany(x => GeneratePermutations(list.Except(new[] {x})), (x, subList) => subList.Union(new[] {x}));
        }
    }
}
