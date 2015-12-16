using System;
using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core
{
    public enum Location
    {
        Venezia,
        Moscow,
        Anxi,
        Karakorum,
        Beijing,
        Samarcanda,
        Kashgar,
        LanZhou,
        Alexandria,
        Ormuz,
        Karachi,
        Xian,
        Adana,
        Kochi,
        Sumatra,
        OasisA,
        OasisB,
        OasisC,
        OasisD,
        OasisE,
        OasisF,
        OasisG,
        OasisH,
        OasisI,
        OasisJ
    }

    public static class Locations
    {
        static Locations()
        {
            All = Enum.GetValues(typeof (Location)).Cast<Location>().ToList();
            Oases = All.Where(x => x.ToString().StartsWith("Oasis"));
        }

        public static List<Location> All { get; }

        public static IEnumerable<Location> Oases { get; }
    }
}
