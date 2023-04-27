using Services.DbContexts;
using Shared.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Services.ModelLoaders
{
    public class CustomerDesiringLoader : BaseModelLoader<CustomerDesiring>, IModelLoader<CustomerDesiring>
    {
        private readonly IDbContext _context;
        public CustomerDesiringLoader(IDbContext context)
        {
            _context = context;
        }

        public bool Accept<TEntity>()
        {
            return typeof(TEntity).Equals(typeof(CustomerDesiring));
        }

        public IEntity LoadOrDefaultAsync<IEntity>(Expression<Func<IEntity, bool>> expression, CancellationToken cancellationToken = default) where IEntity : Entity
        {
            Expression converted = Expression.Convert(expression.Body, typeof(bool));
            var predicate = Expression.Lambda<Func<CustomerDesiring, bool>>(converted, expression.Parameters);
            var entity = _context.SetOf<CustomerDesiring>()
                                .AsQueryable()
                                .Where(d => !d.IsDeleted)
                                .Where(predicate)
                                .Include(d => d.CustomerPrefereds)
                                .ThenInclude(d => d.PreferedProduct)
                                .FirstOrDefault();
            return entity as IEntity;
        }
    }
}
