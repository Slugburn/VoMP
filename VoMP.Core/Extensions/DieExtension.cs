using System;
using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core.Extensions
{
    public static class DieExtension
    {
        public static Die GetLowestDie(this IEnumerable<Die> dice, int atLeast = 1)
        {
            var list = dice as IList<Die> ?? dice.ToList();
            var unassigned = list.Where(d => !d.HasValue).ToList();
            if (unassigned.Any())
                return unassigned.First();
            return list.OrderBy(d => d.SortOrder).FirstOrDefault(d => d.Value >= atLeast);
        }

        public static List<Die> GetLowestDice(this IEnumerable<Die> dice, int count, int atLeast = 1)
        {
            var list = dice as IList<Die> ?? dice.ToList();
            var unassigned = list.Where(d => !d.HasValue).ToList();
            if (unassigned.Any())
                return unassigned.Take(count).ToList();
            return list.Where(d => !d.HasValue || d.Value >= atLeast).OrderBy(d => d.SortOrder).Take(count).ToList();
        }

        public static Die GetHighestDie(this IEnumerable<Die> dice)
        {
            return dice.OrderByDescending(d => d.SortOrder).FirstOrDefault();
        }

        public static List<Die> GetHighestDice(this IEnumerable<Die> dice, int count, Func<Die,bool> condition = null)
        {
            condition = condition ?? (d => true);
            return dice.Where(condition).OrderByDescending(d => d.SortOrder).Take(count).OrderBy(d => d.SortOrder).ToList();
        }

        public static int MinValue(this IEnumerable<Die> dice)
        {
            return dice.Min(d => d.Value);
        }

        public static int MaxValue(this IEnumerable<Die> dice)
        {
            return dice.Max(d => d.HasValue ? d.Value : 6);
        }
    }
}
