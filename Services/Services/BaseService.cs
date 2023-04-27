using Microsoft.EntityFrameworkCore;
using Services.Caching;
using Services.DbContexts;
using Shared.Models;
using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IService<TEntity>
    {
        IQueryable<TEntity> GetAll();
        SearchResult<TEntity> Search(SearchQuery<TEntity> searchQuery, EagerLoadings eagerLoad = null);
        SearchResult<TEntity> Search(IQueryable<TEntity> query, SearchQuery<TEntity> searchQuery);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression, EagerLoadings eagerLoad = null);
        Task<TEntity> LoadAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        Task<Guid> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<List<Guid>> AddAsync(List<TEntity> entities, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, Action<TEntity> updateAction, CancellationToken cancellationToken = default);
        //Task ReplaceAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<int> CountAsync(SearchQuery<TEntity> searchQuery, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }

    public class Service<TEntity> : IService<TEntity> where TEntity : Entity
    {
        protected IDbContext _context;
        protected readonly InMemoryCache _memoryCache;
        private readonly IEnumerable<ICachingLoader> _cachingLoaders;
        public Service(IDbContext context, InMemoryCache memoryCache, IEnumerable<ICachingLoader> cachingLoaders = null)
        {
            _context = context;
            _memoryCache = memoryCache;
            _cachingLoaders = cachingLoaders;
        }

        protected DbSet<TEntity> DbSet
        {
            get
            {
                return _context.SetOf<TEntity>();
            }
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbSet.AsQueryable();
        }
        
        public async Task<TEntity> LoadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var cachingLoader = _cachingLoaders.FirstOrDefault(d => d.Accept<TEntity>());
            return _memoryCache.TryGetAsync(CacheKey.CacheKeyFor<TEntity>(id),
               () => cachingLoader.LoadOrDefaultAsync<TEntity>(_context, d => d.Id == id, cancellationToken)
            );
        }

        public TEntity LoadAsync(string cacheKey, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var cachingLoader = _cachingLoaders.FirstOrDefault(d => d.Accept<TEntity>());
            return _memoryCache.TryGet1Async(cacheKey,
               () => cachingLoader.LoadOrDefaultAsync(_context, predicate, cancellationToken)
            );
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await LoadAsync(id);
            if(entity != null)
            {
                var existingEntity = DbSet.Find(id);
                if (existingEntity != null)
                {
                    //existingEntity.IsDeleted = true;
                    DbSet.Remove(existingEntity);
                    await SaveChangesAsync(cancellationToken);
                }
                _memoryCache.Invalidate<TEntity>();
                //DbSet.Remove(existingEntity);
                _memoryCache.Remove(entity.GetType(), CacheKey.CacheKeyFor<TEntity>(id));
            }
        }

        public async Task ForceDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await LoadAsync(id);
            if(entity != null)
            {
                _memoryCache.Invalidate<TEntity>();
                DbSet.Remove(entity);
                await SaveChangesAsync(cancellationToken);
                _memoryCache.Remove(entity.GetType(), CacheKey.CacheKeyFor<TEntity>(id));
            }
        }

        public async Task<Guid> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            entity.Id = GuidGenerator.NewGuid();
            //entity.CreatedAt = DateTime.UtcNow;
            DbSet.Add(entity);
            if (entity is IIgnoreUTF8NameSearchable ignoreUTF8NameSearchable)
            {
                ignoreUTF8NameSearchable.NameWithoutUTF8 = ignoreUTF8NameSearchable.Name.RemoveSign4VietnameseString();
            }
            await SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task<List<Guid>> AddAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
        {
            var ids = new List<Guid>();
            foreach (var entity in entities)
            {
                entity.Id = GuidGenerator.NewGuid();
                //entity.CreatedAt = DateTime.UtcNow;
                DbSet.Add(entity);
                if (entity is IIgnoreUTF8NameSearchable ignoreUTF8NameSearchable)
                {
                    ignoreUTF8NameSearchable.NameWithoutUTF8 = ignoreUTF8NameSearchable.Name.RemoveSign4VietnameseString();
                }

                ids.Add(entity.Id);
            }
            await SaveChangesAsync(cancellationToken);
            return ids;
        }

        public async Task UpdateAsync(Guid id, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
        {
            var existingEntity = DbSet.Find(id);
            if(existingEntity != null)
            {
                _memoryCache.Invalidate<TEntity>();
                //existingEntity.ModifiedAt = DateTime.UtcNow;
                updateAction(existingEntity);
                if (existingEntity is IIgnoreUTF8NameSearchable ignoreUTF8NameSearchable)
                {
                    ignoreUTF8NameSearchable.NameWithoutUTF8 = ignoreUTF8NameSearchable.Name.RemoveSign4VietnameseString();
                }

                await SaveChangesAsync(cancellationToken);
            }
        }

        public SearchResult<TEntity> Search(SearchQuery<TEntity> searchQuery, EagerLoadings eagerLoadings = null)
        {
            var result = new SearchResult<TEntity>();
            if (searchQuery == null)
            {
                searchQuery = SearchQuery<TEntity>.Default;
            }

            var query = DbSet.AsQueryable().Where(d => !d.IsDeleted);

            if (eagerLoadings != null)
            {
                foreach (var item in eagerLoadings.NavigationCollectionNames)
                {
                    query = query.Include(item);
                }

                foreach (var item in eagerLoadings.NavigationValueNames)
                {
                    query = query.Include(item);
                }
            }

            if (searchQuery.SearchFunc != null)
            {
                query = query.AsEnumerable().Where(entity => searchQuery.SearchFunc.Invoke(entity)).AsQueryable();
                if (typeof(TEntity).Equals(typeof(IIgnoreUTF8NameSearchable)))
                {
                    query = query.AsEnumerable().Where(entity => searchQuery.SearchFunc.Invoke(entity) || (entity as IIgnoreUTF8NameSearchable).NameWithoutUTF8.Contains(searchQuery.Search)).AsQueryable();
                }
                else
                {
                    query = query.AsEnumerable().Where(entity => searchQuery.SearchFunc.Invoke(entity)).AsQueryable();
                }
            }

            if (searchQuery.Filter != null)
            {
                query = query.AsEnumerable().Where(searchQuery.Filter.Predicate).AsQueryable();
            }

            if (searchQuery.IncludeTotal)
            {
                result.Total = query.Count();
            }

            switch (searchQuery.OrderDirection)
            {
                case OrderDirection.Ascending:
                    query = query.OrderBy(searchQuery.Order ?? "CreatedAt");
                    break;
                case OrderDirection.Descending:
                    query = query.OrderByDescending(searchQuery.Order ?? "CreatedAt");
                    break;
                default:
                    break;
            }

            query = query.Skip(searchQuery.Skip)
                         .Take(searchQuery.PageSize);

            result.Data = query.ToList();
            return result;
        }

        public SearchResult<TEntity> Search(IQueryable<TEntity> query, SearchQuery<TEntity> searchQuery)
        {
            var result = new SearchResult<TEntity>();
            if (searchQuery == null)
            {
                searchQuery = SearchQuery<TEntity>.Default;
            }

            if (searchQuery.SearchFunc != null)
            {
                query = query.AsEnumerable().Where(entity => searchQuery.SearchFunc.Invoke(entity)).AsQueryable();
            }

            if (searchQuery.Filter != null)
            {
                query = query.AsEnumerable().Where(searchQuery.Filter.Predicate).AsQueryable();
            }

            if (searchQuery.IncludeTotal)
            {
                result.Total = query.Count();
            }

            query = query.OrderBy(searchQuery.OrderBy ?? "Id")
                         .Skip(searchQuery.Skip)
                         .Take(searchQuery.PageSize);

            result.Data = query.ToList();
            return result;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveAsync(cancellationToken);
        }

        public Task<int> CountAsync(SearchQuery<TEntity> searchQuery, CancellationToken cancellationToken = default)
        {
            if (searchQuery == null)
            {
                searchQuery = SearchQuery<TEntity>.Default;
            }

            var query = DbSet.AsQueryable().Where(d => !d.IsDeleted);

            if (searchQuery.SearchFunc != null)
            {
                query = query.AsEnumerable().Where(entity => searchQuery.SearchFunc.Invoke(entity)).AsQueryable();
            }

            return query.CountAsync();
        }

        public void EnsureDefault(SearchQuery<TEntity> searchQuery)
        {
            if (searchQuery == null)
            {
                searchQuery = SearchQuery<TEntity>.Default;
            }
        }

        //public async Task ReplaceAsync(TEntity entity, CancellationToken cancellationToken = default)
        //{
        //    var oldEntity = await DbSet.FirstOrDefaultAsync(d => d.Id == entity.Id);
        //    _context.Entry(oldEntity).CurrentValues.SetValues(entity);
        //}

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> expression, EagerLoadings eagerLoad = null)
        {
            return await DbSet.Where(d => !d.IsDeleted).FirstOrDefaultAsync(expression);
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            var entity = DbSet.Where(d => !d.IsDeleted).FirstOrDefault(expression);
            if (entity != null)
            {
                DbSet.Remove(entity);
                await SaveChangesAsync(cancellationToken);
            }
        }
    }
}
