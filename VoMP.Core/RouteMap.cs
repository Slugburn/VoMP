using System;
using System.Collections.Generic;
using System.Linq;
using static VoMP.Core.Location;

namespace VoMP.Core
{
    public class RouteMap
    {
        private readonly ILookup<Location, Route> _map;

        private RouteMap(IEnumerable<Route> routes)
        {
            var list = routes as IList<Route> ?? routes.ToList();
            // Add reverse direction
            var reverseDirection = list.Select(r => new Route(r.End, r.Start, r.Cost)).ToList();
            _map = list.Union(reverseDirection).ToLookup(r => r.Start);
        }

        public static RouteMap Standard()
        {
            return new RouteMap(GetStandardRoutes());
        }

        public static RouteMap Oasis()
        {
            var oasisRoutes = GenerateCombinations(new[] {OasisA, OasisB, OasisC, OasisD, OasisE, OasisF, OasisG, OasisH, OasisI, OasisJ})
                .Select(t => new Route(t.Item1, t.Item2));
            var distinctRoutes = GetStandardRoutes().Union(oasisRoutes).GroupBy(x=>x.Start.ToString()+x.End.ToString()).Select(g=>g.First());
            return new RouteMap(distinctRoutes);
        }

        private static List<Route> GetStandardRoutes()
        {
            var routes = new List<Route>
            {
                new Route(Venezia, OasisA),
                new Route(OasisA, Moscow),
                new Route(Moscow, Anxi, Cost.Of.Camel(3)),
                new Route(Anxi, Karakorum, Cost.Of.Camel(2)),
                new Route(Karakorum, OasisB),
                new Route(OasisB, OasisC, Cost.Of.Coin(5)),
                new Route(OasisC, Beijing),
                new Route(OasisB, OasisD),
                new Route(OasisD, Beijing, Cost.Of.Camel(3)),
                new Route(Venezia, OasisE, Cost.Of.Camel(3)),
                new Route(OasisE, Samarcanda),
                new Route(Samarcanda, OasisF, Cost.Of.Camel(2)),
                new Route(OasisF, Kashgar, Cost.Of.Camel(4)),
                new Route(Kashgar, LanZhou, Cost.Of.Camel(2)),
                new Route(LanZhou, OasisD),
                new Route(OasisE, OasisG),
                new Route(Venezia, Alexandria),
                new Route(Alexandria, OasisG, Cost.Of.Coin(7)),
                new Route(OasisG, Ormuz, Cost.Of.Camel(3)),
                new Route(Ormuz, Karachi, Cost.Of.Camel(3)),
                new Route(Karachi, OasisH),
                new Route(OasisH, Xian),
                new Route(Xian, Beijing),
                new Route(Alexandria, OasisI),
                new Route(OasisI, Adana, Cost.Of.Camel(2)),
                new Route(Adana, Kochi, Cost.Of.Coin(15)),
                new Route(Kochi, OasisH, Cost.Of.Camel(4)),
                new Route(Kochi, Sumatra, Cost.Of.Coin(10)),
                new Route(Sumatra, OasisJ, Cost.Of.Camel(4)),
                new Route(OasisJ, Beijing)
            };
            return routes;
        }

        public List<Route> ShortestPath(Location start, Location end)
        {
            var dist = new Dictionary<Location, int>();
            var prev = new Dictionary<Location, Location>();
            foreach (var loc in Locations.All)
                dist[loc] = int.MaxValue;
            dist[start] = 0;
            var Q = new HashSet<Location>(Locations.All);
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
                foreach (var route in _map[u])
                {
                    var v = route.End;
                    var alt = dist[u] + (3 + route.Cost.Coin + route.Cost.Camel *2);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }
            throw new Exception("No path found");
        }

        public List<Route> ShortestPath(IList<Location> locations)
        {
            var list = new List<Route>();
            for (var i = 0; i < locations.Count() - 1; i++)
            {
                list.AddRange(ShortestPath(locations[i], locations[i + 1]));
            }
            return list;
        }

        public List<Route> BestPath(Location start, IEnumerable<Location> targets)
        {
            var permutations = GeneratePermutations(targets).ToList();
            return permutations
                .Select(ts => ShortestPath(new[] {start}.Union(ts).ToList()))
                .OrderBy(p => p.Count)
                .First();
        }

        public static IEnumerable<IEnumerable<T>> GeneratePermutations<T>(IEnumerable<T> enumerable)
        {
            var list = enumerable as IList<T> ?? enumerable.ToList();
            if (list.Count == 1) return new[] {list};
            return list.SelectMany(x => GeneratePermutations(list.Except(new[] {x})), (x, subList) => subList.Union(new[] {x}));
        }

        public static IEnumerable<Tuple<T, T>> GenerateCombinations<T>(IEnumerable<T> enumerable)
        {
            var list = enumerable as IList<T> ?? enumerable.ToList();
            for (var i = 0; i < list.Count - 1; i++)
                for (var j = i + 1; j < list.Count; j++)
                    yield return new Tuple<T, T>(list[i], list[j]);
        }
    }
}