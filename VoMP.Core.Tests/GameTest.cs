using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace VoMP.Core.Tests
{
    [TestFixture]
    public class GameTest
    {
        [Test]
        public void SetUp()
        {
            var g = new Game();
            try
            {
                g.SetUp();
                g.StartGame();
            }
            finally
            {
                Console.WriteLine(g);
            }
        }
    }
}
