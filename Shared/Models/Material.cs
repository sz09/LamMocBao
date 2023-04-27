using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Materials")]
    public class Material : Entity, IEntity
    {
        [StringLength(200)]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    [Serializable]
    [Table("ProductMaterials")]
    public class ProductMaterial : Entity, IEntity
    {
        public Guid ProductId { get; set; }
        public Guid MaterialId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}
