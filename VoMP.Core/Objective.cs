using static VoMP.Core.Location;

namespace VoMP.Core
{
    public class Objective
    {
        public Objective(Location location1, Location location2, int vp)
        {
            Location1 = location1;
            Location2 = location2;
            Vp = vp;
        }

        public Location Location1 { get; set; }
        public Location Location2 { get; set; }
        public int Vp { get; set; }

        public static Objective[] CreateAll()
        {
            return new[]
            {
                new Objective(Alexandria, Anxi, 4),
                new Objective(LanZhou, Kochi, 4),
                new Objective(Karakorum, Xian, 4),
                new Objective(Moscow, Xian, 4),
                new Objective(Samarcanda, Xian, 4),
                new Objective(Alexandria, Kashgar, 5),
                new Objective(Karachi, Kashgar, 5),
                new Objective(Karakorum, Kochi, 5),
                new Objective(LanZhou, Adana, 5),
                new Objective(Moscow, Ormuz, 5),
                new Objective(Samarcanda, Adana, 5),
                new Objective(LanZhou, Ormuz, 6),
                new Objective(Sumatra, Ormuz, 6),
                new Objective(Karachi, Anxi, 7),
                new Objective(Karakorum, Adana, 7),
                new Objective(Moscow, Kashgar, 7),
                new Objective(Samarcanda, Kochi, 7),
                new Objective(Sumatra, Anxi, 8)
            };
        }
    }
}