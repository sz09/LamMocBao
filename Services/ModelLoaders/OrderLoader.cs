using Microsoft.EntityFrameworkCore;
using Services.DbContexts;
using Shared.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Services.ModelLoaders
{
    public class OrderLoader : BaseModelLoader<Order>, IModelLoader<Order>
    {
        private readonly IDbContext _context;
        public OrderLoader(IDbContext context)
        {
            _context = context;
        }

        public bool Accept<TEntity>()
        {
            return typeof(TEntity).Equals(typeof(Order));
        }

        public IEntity LoadOrDefaultAsync<IEntity>(Expression<Func<IEntity, bool>> expression, CancellationToken cancellationToken = default) where IEntity : Entity
        {
            Expression converted = Expression.Convert(expression.Body, typeof(bool));
            var predicate = Expression.Lambda<Func<Order, bool>>(converted, expression.Parameters);
            var entity = _context.SetOf<Order>()
                                .AsQueryable()
                                .Where(d => !d.IsDeleted)
                                .Where(predicate)
                                .Include(d => d.Customer)
                                .FirstOrDefault();
            return entity as IEntity;
        }
    }
}
