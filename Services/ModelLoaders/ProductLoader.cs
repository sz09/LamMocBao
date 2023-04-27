using Services.DbContexts;
using Shared.Models;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Services.ModelLoaders
{
    public class ProductLoader : BaseModelLoader<Product>, IModelLoader<Product>
    {
        private readonly IDbContext _context;
        public ProductLoader(IDbContext context)
        {
            _context = context;
        }

        public bool Accept<TEntity>()
        {
            return typeof(TEntity).Equals(typeof(Product));
        }

        public IEntity LoadOrDefaultAsync<IEntity>(Expression<Func<IEntity, bool>> expression, CancellationToken cancellationToken = default) where IEntity : Entity
        {
            Expression converted = Expression.Convert(expression.Body, typeof(bool));
            var predicate = Expression.Lambda<Func<Product, bool>>(converted, expression.Parameters);
            var entity = _context.SetOf<Product>()
                                .AsQueryable()
                                .Where(d => !d.IsDeleted)
                                .Where(predicate)
                                .Include(d => d.ProductImages.Where(d => !d.IsDeleted))
                                .Include(d => d.ProductTags)
                                .ThenInclude(d => d.Tag)
                                .Include(d => d.ProductSizes.Where(d => !d.IsDeleted))
                                .ThenInclude(d => d.Size)
                                .Include(d => d.ProductMaterials.Where(d => !d.IsDeleted))
                                .ThenInclude(d => d.Material)
                                .Include(d => d.ProductCategories.Where(d => !d.IsDeleted))
                                .ThenInclude(d => d.Category)
                                ////.Include(d => d.CustomerPrefereds.Where(d => !d.IsDeleted))
                                ////.ThenInclude(d => d.CustomerDesiring)
                                .FirstOrDefault();
            return entity as IEntity;
        }
    }
}
