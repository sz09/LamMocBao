using Microsoft.EntityFrameworkCore;
using Services.DbContexts;
using Shared.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Services.ModelLoaders
{
    public class NewsPaperPostPostLoader : BaseModelLoader<NewsPaperPost>, IModelLoader<NewsPaperPost>
    {
        private readonly IDbContext _context;
        public NewsPaperPostPostLoader(IDbContext context)
        {
            _context = context;
        }

        public bool Accept<TEntity>()
        {
            return typeof(TEntity).Equals(typeof(NewsPaperPost));
        }

        public IEntity LoadOrDefaultAsync<IEntity>(Expression<Func<IEntity, bool>> expression, CancellationToken cancellationToken = default) where IEntity : Entity
        {
            Expression converted = Expression.Convert(expression.Body, typeof(bool));
            var predicate = Expression.Lambda<Func<NewsPaperPost, bool>>(converted, expression.Parameters);
            var entity = _context.SetOf<NewsPaperPost>()
                                .AsQueryable()
                                .Where(d => !d.IsDeleted)
                                .Where(predicate)
                                .Include(d => d.UploadedImage)
                                .FirstOrDefault();
            return entity as IEntity;
        }
    }
}
