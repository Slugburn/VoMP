using System;
using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core.Extensions
{
    public static class DieExtension
    {
        public static Die GetLowest(this IEnumerable<Die> dice, Func<Die, bool> condition = null)
        {
            return condition==null ? dice.OrderBy(d => d.Value).FirstOrDefault() : dice.OrderBy(d => d.Value).FirstOrDefault(condition);
        }

        public static List<Die> GetLowest(this IEnumerable<Die> dice, int count, Func<Die, bool> condition)
        {
            return dice.Where(condition).OrderBy(d => d.Value).Take(count).ToList();
        }

        public static Die GetHighest(this IEnumerable<Die> dice)
        {
            return dice.OrderByDescending(d => d.Value).FirstOrDefault();
        }

        public static List<Die> GetHighest(this IEnumerable<Die> dice, int count, Func<Die,bool> condition)
        {
            return dice.Where(condition).OrderByDescending(d => d.Value).Take(count).OrderBy(d => d.Value).ToList();
        }

        public static int MinValue(this IEnumerable<Die> dice)
        {
            return dice.Min(d => d.Value);
        }

        public static int MaxValue(this IEnumerable<Die> dice)
        {
            return dice.Max(d => d.Value);
        }
    }
}
