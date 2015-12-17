using System;
using System.Linq;
using static VoMP.Core.Location;

namespace VoMP.Core
{
    public static class Locations
    {
        static Locations()
        {
            All = Enum.GetValues(typeof (Location)).Cast<Location>().ToArray();
            Oases = All.Where(x => x.ToString().StartsWith("Oasis")).ToArray();
            SmallCities = new[] {Anxi, Kashgar, Ormuz, Xian, Adana, Kochi};
            LargeCities = new[] {Moscow, Karakorum, Samarcanda, LanZhou, Alexandria, Karachi, Sumatra};
        }

        public static Location[] LargeCities { get; set; }

        public static Location[] All { get; }

        public static Location[] Oases { get; }

        public static Location[] SmallCities { get; }
    }
}