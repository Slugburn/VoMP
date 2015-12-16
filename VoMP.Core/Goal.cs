using static VoMP.Core.Location;

namespace VoMP.Core
{
    public class Goal
    {
        static Goal()
        {
            Catalogue = CreateGoals();
        }

        public Goal(Location location1, Location location2, int vp)
        {
            Location1 = location1;
            Location2 = location2;
            Vp = vp;
        }

        public Location Location1 { get; set; }
        public Location Location2 { get; set; }
        public int Vp { get; set; }
        public static Goal[] Catalogue { get; }

        private static Goal[] CreateGoals()
        {
            return new[]
            {
                new Goal(Alexandria, Anxi, 4),
                new Goal(LanZhou, Kochi, 4),
                new Goal(Karakorum, Xian, 4),
                new Goal(Moscow, Xian, 4),
                new Goal(Samarcanda, Xian, 4),
                new Goal(Alexandria, Kashgar, 5),
                new Goal(Karachi, Kashgar, 5),
                new Goal(Karakorum, Kochi, 5),
                new Goal(LanZhou, Adana, 5),
                new Goal(Moscow, Ormuz, 5),
                new Goal(Samarcanda, Adana, 5),
                new Goal(LanZhou, Ormuz, 6),
                new Goal(Sumatra, Ormuz, 6),
                new Goal(Karachi, Anxi, 7),
                new Goal(Karakorum, Adana, 7),
                new Goal(Moscow, Kashgar, 7),
                new Goal(Samarcanda, Kochi, 7),
                new Goal(Sumatra, Anxi, 8)
            };
        }
    }
}