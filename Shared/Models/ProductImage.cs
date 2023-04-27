using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("ProductImages")]
    public class ProductImage: Entity, IEntity
    {
        public Guid? ProductId { get; set; }
        public string Url { get; set; }
        public string UrlPreview { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
