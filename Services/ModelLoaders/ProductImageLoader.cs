using Services.DbContexts;
using Shared.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Services.ModelLoaders
{
    public class ProductImageLoader : BaseModelLoader<ProductImage>, IModelLoader<ProductImage>
    {
        private readonly IDbContext _context;
        public ProductImageLoader(IDbContext context)
        {
            _context = context;
        }

        public bool Accept<TEntity>()
        {
            return typeof(TEntity).Equals(typeof(ProductImage));
        }

        public IEntity LoadOrDefaultAsync<IEntity>(Expression<Func<IEntity, bool>> expression, CancellationToken cancellationToken = default) where IEntity : Entity
        {
            Expression converted = Expression.Convert(expression.Body, typeof(bool));
            var predicate = Expression.Lambda<Func<ProductImage, bool>>(converted, expression.Parameters);
            var entity = _context.SetOf<ProductImage>()
                                .AsQueryable()
                                .Where(d => !d.IsDeleted)
                                .Where(predicate)
                                .Include(d => d.Product)
                                .FirstOrDefault();
            return entity as IEntity;
        }
    }
}
