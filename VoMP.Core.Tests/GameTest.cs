using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using NUnit.Framework;
using VoMP.Core.Extensions;

namespace VoMP.Core.Tests
{
    [TestFixture]
    public class GameTest
    {
        [Test]
        public void RunGame()
        {
            XmlConfigurator.Configure();
            var g = new Game();
            try
            {
                g.SetUp();
                g.StartGame();
            }
            finally
            {
                Console.WriteLine(g);
                Console.WriteLine(g.GetPlayers().Select(p=>$"{p.Color} = {p.Resources.Vp}").ToDelimitedString(", "));
            }
        }
    }
}
