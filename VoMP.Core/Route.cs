namespace VoMP.Core
{
    public class Route
    {
        public Route(Location start, Location end)
        {
            Start = start;
            End = end;
        }

        public Location Start { get; set; }
        public Location End { get; set; }
        public int Camel { get; set; }
        public int Coin { get; set; }
    }
}
