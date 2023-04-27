using System;
using System.Linq.Expressions;

namespace Shared.Utilities
{
    public class NameCollector<T>
    {
        public static string Get<TProperty>(Expression<Func<T, TProperty>> expr)
        {
            var name = expr.Parameters[0].Name;
            return expr.ToString().Replace($"{name} => {name}.", string.Empty);
        }
    }
}
