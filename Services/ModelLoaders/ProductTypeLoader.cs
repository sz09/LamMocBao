using Microsoft.EntityFrameworkCore;
using Services.DbContexts;
using Shared.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Services.ModelLoaders
{
    public class ProductTypeLoader : BaseModelLoader<ProductType>, IModelLoader<ProductType>
    {
        private readonly IDbContext _context;
        public ProductTypeLoader(IDbContext context)
        {
            _context = context;
        }

        public bool Accept<TEntity>()
        {
            return typeof(TEntity).Equals(typeof(ProductType));
        }

        public IEntity LoadOrDefaultAsync<IEntity>(Expression<Func<IEntity, bool>> expression, CancellationToken cancellationToken = default) where IEntity : Entity
        {
            Expression converted = Expression.Convert(expression.Body, typeof(bool));
            var predicate = Expression.Lambda<Func<ProductType, bool>>(converted, expression.Parameters);
            var entity = _context.SetOf<ProductType>()
                                .AsQueryable()
                                .Where(d => !d.IsDeleted)
                                .Where(predicate)
                                .Include(d => d.ProductTypeTags)
                                .ThenInclude(d => d.Tag)
                                .FirstOrDefault();
            return entity as IEntity;
        }
    }
}
