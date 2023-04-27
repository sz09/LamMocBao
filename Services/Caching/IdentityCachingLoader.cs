using Services.DbContexts;
using Shared.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Services.Caching
{
    public class IdentityCachingLoader : ICachingLoader
    {
        public bool Accept<TEntity>() where TEntity : Entity
        {
            return typeof(TEntity).Namespace.EndsWith("Identify");
        }

        public TEntity LoadOrDefaultAsync<TEntity>(IDbContext context, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : Entity
        {
            return context.SetOf<TEntity>().FirstOrDefault(predicate);
        }
    }
}
