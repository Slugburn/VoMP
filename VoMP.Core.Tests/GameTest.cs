using System;
using System.Linq;
using log4net.Config;
using NUnit.Framework;
using VoMP.Core.Characters;
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
                g.SetUp(Character.CreateBasic());
                g.StartGame();
            }
            finally
            {
                Console.WriteLine(g);
                Console.WriteLine(g.GetPlayers().Select(p=>$"{p.Color} ({p.Character.Name}) = {p.Resources.Vp}").ToDelimitedString(", "));
            }
        }
    }
}
