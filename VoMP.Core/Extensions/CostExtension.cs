using System.Collections.Generic;
using System.Linq;

namespace VoMP.Core.Extensions
{
    public static class CostExtension
    {
        public static Cost Total(this IEnumerable<Cost> costs)
        {
            return costs.Aggregate(Cost.None, (total, cost) => total.Add(cost));
        }
    }
}
