using System.Collections.Generic;
using System.Linq;

namespace LamMocBaoWeb.Utilities
{
    public static class IEnumerableUtils
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items, int maxItems)
        {
            return (from x in items.Select((T item, int inx) => new { item, inx })
                   group x by x.inx / maxItems into g
                   select from x in g
                          select x.item).ToList();
        }
    }
}
