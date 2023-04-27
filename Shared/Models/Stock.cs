using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Stocks")]
    public class Stock : Entity, IEntity
    {
        public Guid ProductId { get; set; }
        public Guid ProductSizeId { get; set; }
        public int Quantity { get; set; }
    }

    [Serializable]
    [Table("StockHistories")]
    public class StockHistory : Entity, IEntity
    {
        public Guid ProductId { get; set; }
        public Guid ProductSizeId { get; set; }
        public int Quantity { get; set; }


        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("ProductSizeId")]
        public virtual ProductSize ProductSize { get; set; }
    }
}
