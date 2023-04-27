using Services.DbContexts;
using Shared.Models;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Caching
{
    public interface ICachingLoader
    {
        bool Accept<TEntity>() where TEntity : Entity;
        TEntity LoadOrDefaultAsync<TEntity>(IDbContext context, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : Entity;
    }
}
