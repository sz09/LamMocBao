using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Services.Caching;
using Services.Services;
using Shared.Models;
using Shared.Models.Identify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.DbContexts
{
    public class ApplicationDbContext : DbContext, IDbContext
    {
        private readonly InMemoryCache _memoryCache;
        private readonly IServiceConfig _serviceConfig;
        public ApplicationDbContext(InMemoryCache memoryCache, IServiceConfig serviceConfig) 
            : base()
        {
            _memoryCache = memoryCache;
            _serviceConfig = serviceConfig;
        }
        public DbSet<TEntity> SetOf<TEntity>() where TEntity : Entity
        {
            return Set<TEntity>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_serviceConfig.ConnectionString);
        }

        public new EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : Entity
        {
            return base.Entry<TEntity>(entity);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CustomerDesiring> CustomerDesirings { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<CustomerPreferedInfos> CustomerPreferedInfos { get; set; }
        public DbSet<UploadedImage> UploadedImages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<NewsPaperPost> NewsPaperPosts { get; set; }
        public DbSet<CustomerComment> CustomerComments { get; set; }
        public DbSet<ProductSubType> ProductSubTypes { get; set; }
        public DbSet<Knowledge> Knowledges { get; set; }
        public DbSet<PublishedKnowledge> PublishedKnowledges { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<ProductMaterial> ProductMaterials { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public DbSet<Analysis> Analysises { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<HighlightItem> HighlightItems { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            var sessionCacheTypesCleared = new List<string>();
            var changes = ChangeTracker.Entries().GroupBy(d => d.Entity.GetType());
            foreach (var change in changes)
            {
                _memoryCache.Invalidate(change.Key, sessionCacheTypesCleared);
            }
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
