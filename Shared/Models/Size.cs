using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Sizes")]
    public class Size : Entity, IEntity
    {
        public double Number { get; set; }
        public string Unit { get; set; }
    }

    [Serializable]
    [Table("ProductSizes")]
    public class ProductSize : Entity, IEntity
    {
        public Guid ProductId { get; set; }
        public Guid SizeId { get; set; }
        public decimal? SellingPrice { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("SizeId")]
        public virtual Size Size { get; set; }
    }
}
