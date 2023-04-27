using Services.DbContexts;
using System.Data.Entity.Migrations;

namespace Services.DbContexts
{
    #region MyRegion
    public class LamMocBaoDbMigrations : DbMigration
    {
        public override void Up()
        {
            MigrateTag();
            MigrateSize();
            MigrateProduct();
        }

        #region Up
        private void MigrateTag()
        {
            CreateTable("dbo.Tags",
                d => new
                {
                    Id = d.Guid(),
                    ProductId = d.Guid(),
                    Name = d.String(),
                    CreatedAt = d.DateTime(),
                    UpdatedAt = d.DateTime(),
                    CreateBy = d.String(),
                    ModifiedBy = d.String(),
                    IsDeleted = d.Boolean()
                })
                .PrimaryKey(d => d.Id)
                .ForeignKey("dbo.Products", d => d.ProductId, cascadeDelete: true);
        }

        private void MigrateSize()
        {
            CreateTable("dbo.Sizes",
                d => new
                {
                    Id = d.Guid(),
                    ProductId = d.Guid(),
                    Number = d.Double(),
                    Unit = d.String(),
                    CreatedAt = d.DateTime(),
                    UpdatedAt = d.DateTime(),
                    CreateBy = d.String(),
                    ModifiedBy = d.String(),
                    IsDeleted = d.Boolean()
                })
                .PrimaryKey(d => d.Id)
                .ForeignKey("dbo.Products", d => d.ProductId, cascadeDelete: true);
        }

        private void MigrateProduct()
        {
            AddColumn("dbo.Products", "SellingPrice", d => d.Decimal());
            AddColumn("dbo.Products", "PurchasingPrice", d => d.Decimal());
        }
        #endregion
    }
    #endregion

    //public class Configuration: DbMigrationsConfiguration<ApplicationDbContext>
    //{
    //    public Configuration()
    //    {
    //        AutomaticMigrationsEnabled = true;
    //    }
    //}
}
