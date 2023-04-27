using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Models;
using Shared.Models.Identify;
using System.Threading;
using System.Threading.Tasks;

namespace Services.DbContexts
{
    public interface IDbContext
    {
        Task SaveAsync(CancellationToken cancellationToken = default);
        DbSet<TEntity> SetOf<TEntity>() where TEntity : Entity;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : Entity;
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<CustomerDesiring> CustomerDesirings { get; set; }
        DbSet<ProductImage> ProductImages { get; set; }
        DbSet<CustomerPreferedInfos> CustomerPreferedInfos { get; set; }
        DbSet<UploadedImage> UploadedImages { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Size> Sizes { get; set; }
        DbSet<ProductType> ProductTypes { get; set; }
        DbSet<ProductTag> ProductTags { get; set; }
        DbSet<ProductSize> ProductSizes { get; set; }
        DbSet<SystemSetting> SystemSettings { get; set; }
        DbSet<NewsPaperPost> NewsPaperPosts { get; set; }
        DbSet<CustomerComment> CustomerComments { get; set; }
        DbSet<ProductSubType> ProductSubTypes { get; set; }
        DbSet<Knowledge> Knowledges { get; set; }
        DbSet<PublishedKnowledge> PublishedKnowledges { get; set; }
        DbSet<Cart> Carts { get; set; }
        DbSet<Promotion> Promotions { get; set; }
        DbSet<Material> Materials { get; set; }
        DbSet<ProductMaterial> ProductMaterials { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<OrderDetail> OrderDetails { get; set; }
        DbSet<Customer> Customers { get; set; }
        DbSet<Appointment> Appointments { get; set; }
        DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        DbSet<Analysis> Analysises { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<ProductCategory> ProductCategories { get; set; }
        DbSet<HighlightItem> HighlightItems { get; set; }
        DbSet<Stock> Stocks { get; set; }
    }
}
