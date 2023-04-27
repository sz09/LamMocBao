using Shared.Models;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Services.ModelLoaders
{
    public interface IModelLoader
    {
        bool Accept<TEntity>();
        IEntity LoadOrDefaultAsync<IEntity>(Expression<Func<IEntity, bool>> predicate, CancellationToken cancellationToken = default) where IEntity : Entity;
    }

    public interface IModelLoader<TEntity> : IModelLoader where TEntity : Entity { }

    public class BaseModelLoader<TEntity> { }
}
