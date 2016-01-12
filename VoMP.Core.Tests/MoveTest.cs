using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace VoMP.Core.Tests
{
    [TestFixture]
    public class MoveTest
    {
        [Test]
        public void  MoveTwoPawns()
        {
            var path = new[]
            {
                new Route(Location.Venezia, Location.Alexandria),
                new Route(Location.Venezia, Location.OasisA),
                new Route(Location.OasisA, Location.Moscow),
            };
            new TestScenario()
                .AddPawn()
                .Move(path)
                .VerifyPawnLocations(Location.Alexandria, Location.Moscow)
                .VerifyTradingPosts(Location.Alexandria, Location.Moscow);
        }
    }
}
