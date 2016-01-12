using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace VoMP.Core.Extensions
{
    public static class EnumerableExtension
    {
        private static readonly Random Random = new Random();

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action )
        {
            source.ToList().ForEach(action);
        }

        public static IEnumerable<IEnumerable<T>> Segment<T>(this IEnumerable<T> enumerable, int segmentSize)
        {
            if (segmentSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(segmentSize), segmentSize, "segmentSize must be larger than zero");

            if (enumerable == null)
                return null;

            var idx = 0;
            return enumerable.GroupBy(
                item => (Interlocked.Increment(ref idx) - 1) / segmentSize,
                item => item);
        }


        public static string ToDelimitedString<T>(this IEnumerable<T> source, string seperator = ",")
        {
            return String.Join(seperator, source);
        }

        public static List<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var list = new List<T>(source);
            var shuffled = new List<T>();
            while (list.Count > 0)
            {
                var randomIndex = Random.Next(list.Count);
                shuffled.Add(list[randomIndex]);
                list.RemoveAt(randomIndex);
            }
            return shuffled;
        }

        public static List<Tuple<T1, T2>> PairWith<T1, T2>(this IEnumerable<T1> source1, IEnumerable<T2> source2)
        {
            return source1.Zip(source2, (x, y) => new Tuple<T1, T2>(x, y)).ToList();
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

        public static IEnumerable<List<T>> SegmentOnChange<T>(this IEnumerable<T> list, Func<T, T, bool> segmentCondition)
        {
            var subList = new List<T>();
            foreach (var item in list)
            {
                if (subList.Any() && segmentCondition(subList.Last(), item))
                {
                    yield return subList;
                    subList = new List<T> {item};
                }
                else
                {
                    subList.Add(item);
                }
            }
            yield return subList;
        }
    }
}
