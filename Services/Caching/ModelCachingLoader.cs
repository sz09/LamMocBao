using Services.DbContexts;
using Services.ModelLoaders;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Services.Caching
{
    public class ModelCachingLoader : ICachingLoader
    {
        private readonly IEnumerable<IModelLoader> _modelLoaders;
        public ModelCachingLoader(IEnumerable<IModelLoader> modelLoaders)
        {
            _modelLoaders = modelLoaders;
        }

        public bool Accept<TEntity>() where TEntity : Entity
        {
            return !typeof(TEntity).Namespace.EndsWith("Identify");
        }

        public TEntity LoadOrDefaultAsync<TEntity>(IDbContext context, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : Entity
        {
            var handler = _modelLoaders.FirstOrDefault(d => d.Accept<TEntity>());
            return handler.LoadOrDefaultAsync(predicate, cancellationToken);
        }
    }
}
