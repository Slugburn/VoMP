using System;
using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core.Extensions
{
    public static class DieExtension
    {
        public static Die GetLowestDie(this IEnumerable<Die> dice, int atLeast = 1)
        {
            return dice.OrderBy(d => d.Value).FirstOrDefault(d => d.Value >= atLeast);
        }

        public static List<Die> GetLowestDice(this IEnumerable<Die> dice, int count, int atLeast = 1)
        {
            return dice.Where(d => d.Value >= atLeast).OrderBy(d => d.Value).Take(count).ToList();
        }

        public static Die GetHighestDie(this IEnumerable<Die> dice)
        {
            return dice.OrderByDescending(d => d.Value).FirstOrDefault();
        }

        public static List<Die> GetHighestDice(this IEnumerable<Die> dice, int count, Func<Die,bool> condition = null)
        {
            condition = condition ?? (d => true);
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
