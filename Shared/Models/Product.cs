using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Products")]
    public class Product: Entity, IEntity, IIgnoreUTF8NameSearchable
    {
        [Required]
        [Column(TypeName = "VARCHAR")]
        [StringLength(200)]
        public string LinkName { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(200)]
        public string NameWithoutUTF8 { get; set; }
        // Mean formatteed
        public string Description { get; set; }
        public string FormattedInfomations { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? PurchasingPrice { get; set; }
        public Guid? ProductTypeId { get; set; }
        //public Guid? CategoryId { get; set; }
        public string ProductFrom { get; set; }
        public int SoldNumberCount { get; set; }
        public virtual ICollection<ProductSize> ProductSizes { get; set; }
        public virtual ICollection<ProductTag> ProductTags { get; set; }
        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; }
        [ForeignKey("ProductTypeId")]
        public virtual ProductType ProductType { get; set; }
        //[ForeignKey("CategoryId")]
        //public virtual Category Category { get; set; }
        public virtual ICollection<CustomerPreferedInfos> CustomerPrefereds { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
