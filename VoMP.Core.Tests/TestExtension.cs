using System;
using System.Linq;

namespace VoMP.Core.Tests
{
    public static class TestExtension
    {
        public static Location GetMapLocation(this Game game, Func<MapLocation, bool> condition)
        {
            return game.GetMapLocations().Single(condition).Location;
        }

        public static Location GetCityBonusLocation(this Game game, Func<Reward, bool> condition)
        {
            return game.GetMapLocation(l=>l.CityBonus != null && condition(l.CityBonus.Reward));
        }

        public static void Zero(this ResourceBag resources)
        {
            resources.Coin -= resources.Coin;
            resources.Camel -= resources.Camel;
            resources.Gold -= resources.Gold;
            resources.Pepper -= resources.Pepper;
            resources.Silk -= resources.Silk;
            resources.Vp -= resources.Vp;
        }
    }
}
