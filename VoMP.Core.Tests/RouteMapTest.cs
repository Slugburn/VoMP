using System;
using System.Linq;
using NUnit.Framework;
using static VoMP.Core.Location;

namespace VoMP.Core.Tests
{
    [TestFixture]
    public class RouteMapTest
    {
        [Test]
        public void ShortestPath()
        {
            var shortest = RouteMap.Standard().ShortestPath(Venezia, Beijing);
            Assert.That(shortest.First().Start, Is.EqualTo(Venezia));
            Assert.That(shortest.Select(x => x.End), Is.EquivalentTo(new[] {OasisA, Moscow, Anxi, Karakorum, OasisB, OasisC, Beijing}));
        }

        [Test]
        public void BestPath()
        {
            var bestPath = RouteMap.Standard().BestPath(Venezia, new[] { Beijing, Sumatra, Anxi, Moscow, Kashgar });
            var endpoints = bestPath.Select(r => r.End).ToList();
            Console.WriteLine(string.Join(",",endpoints));
            Console.WriteLine("{0} Moves", bestPath.Count);
            Console.WriteLine("{0} Camels", bestPath.Sum(r=>r.Camel));
            Console.WriteLine("{0} Coins", bestPath.Sum(r=>r.Coin));
        }

        [Test]
        public void BestPathWithOasisMovement()
        {
            var bestPath = RouteMap.Oasis().BestPath(Venezia, new [] { Beijing, Sumatra, Anxi, Moscow, Kashgar });
            var endpoints = bestPath.Select(r => r.End).ToList();
            Console.WriteLine(string.Join(",", endpoints));
            Console.WriteLine("{0} Moves", bestPath.Count);
            Console.WriteLine("{0} Camels", bestPath.Sum(r => r.Camel));
            Console.WriteLine("{0} Coins", bestPath.Sum(r => r.Coin));
        }
    }
}
