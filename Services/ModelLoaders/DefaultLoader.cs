using Services.DbContexts;
using Shared.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Services.ModelLoaders
{
    public class DefaultLoader : BaseModelLoader<Entity>, IModelLoader<Entity>
    {
        private readonly IDbContext _context;
        public DefaultLoader(IDbContext context)
        {
            _context = context;
        }

        public bool Accept<TEntity>()
        {
            return true;
        }

        public IEntity LoadOrDefaultAsync<IEntity>(Expression<Func<IEntity, bool>> filterExpression, CancellationToken cancellationToken = default) where IEntity : Entity
        {
            var entity = _context.SetOf<IEntity>()
                                .AsQueryable()
                                .Where(d => !d.IsDeleted)
                                .Where(filterExpression)
                                .FirstOrDefault();
            return entity as IEntity;
        }
    }
}
