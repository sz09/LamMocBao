using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("ProductTypes")] // Vật phẩm theo ngũ hành
    public class ProductType : Entity, IEntity
    {
        [StringLength(200)]
        public string Name { get; set; }
        public int SequenceNumber { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(200)]
        public string LinkName { get; set; }
        public string DisplayImageUrl { get; set; }
        public string Description { get; set; }

        //public virtual ICollection<ProductSubType> ProductSubTypes { get; set; }
        public virtual ICollection<ProductTypeTag> ProductTypeTags { get; set; }
    }

    [Table("ProductSubTypes")] // Cấp nhỏ vật phẩm theo ngũ hành
    public class ProductSubType : Entity, IEntity
    {
        public Guid ProductTypeId { get; set; }
        public string Name { get; set; }
        public int SequenceNumber { get; set; }

        //[ForeignKey("ProductTypeId")]
        //public virtual ProductType ProductType { get; set; }
    }
}
