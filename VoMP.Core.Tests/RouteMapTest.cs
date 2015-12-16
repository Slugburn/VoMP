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
            var shortest = RouteMap.ShortestPath(Venezia, Xian);
            Assert.That(shortest.Count, Is.EqualTo(6));
            Assert.That(shortest.First().Start, Is.EqualTo(Venezia));
            Assert.That(shortest.Select(x => x.End), Is.EquivalentTo(new[] {Alexandria, OasisG, Ormuz, Karachi, OasisH, Xian}));
        }

        [Test]
        public void BestPath()
        {
            var bestPath = RouteMap.BestPath(Beijing, Sumatra, Anxi, Moscow, Kashgar);
            var endpoints = bestPath.Select(r => r.End).ToList();
            Console.WriteLine(string.Join(",",endpoints));
            Console.WriteLine("{0} Moves", bestPath.Count);
            Console.WriteLine("{0} Camels", bestPath.Sum(r=>r.Camel));
            Console.WriteLine("{0} Coins", bestPath.Sum(r=>r.Coin));
        }
    }
}
