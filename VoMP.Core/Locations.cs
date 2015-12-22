using System;
using System.Linq;
using static VoMP.Core.Location;

namespace VoMP.Core
{
    public static class Locations
    {
        static Locations()
        {
            All = Enum.GetValues(typeof (Location)).Cast<Location>().Except(new[] {Unknown}).ToArray();
            Oases = All.Where(x => x.IsOasis()).ToArray();
            SmallCities = new[] {Anxi, Kashgar, Ormuz, Xian, Adana, Kochi};
            LargeCities = new[] {Moscow, Karakorum, Samarcanda, LanZhou, Alexandria, Karachi, Sumatra};
        }

        public static Location[] LargeCities { get; set; }

        public static Location[] All { get; }

        public static Location[] Oases { get; }

        public static Location[] SmallCities { get; }

        public static bool IsOasis(this Location location)
        {
            return location.ToString().StartsWith("Oasis");
        }

        public static bool IsTradeLocation(this Location location)
        {
            return !IsOasis(location) && location != Venezia;
        }
    }
}