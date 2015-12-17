using System;
using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core
{
    public static class CardExtension
    {
        private static readonly Random Random = new Random();

        public static List<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var list = source as List<T> ?? new List<T>(source);
            var shuffled = new List<T>();
            while (list.Count > 0)
            {
                var randomIndex = Random.Next(list.Count);
                shuffled.Add(list[randomIndex]);
                list.RemoveAt(randomIndex);
            }
            return shuffled;
        }

        public static List<Tuple<T1, T2>> PairWithRandom<T1, T2>(this IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            return source1.Shuffle().Zip(source2.Shuffle(), (x, y) => new Tuple<T1, T2>(x, y)).ToList();
        }

        public static List<T> Draw<T>(this IList<T> source, int count)
        {
            var drawn = source.Take(count).ToList();
            for (var i = 0; i < drawn.Count; i++)
                source.RemoveAt(0);
            return drawn;
        }

        public static T Draw<T>(this IList<T> source)
        {
            return Draw(source, 1).FirstOrDefault();
        }
    }
}
