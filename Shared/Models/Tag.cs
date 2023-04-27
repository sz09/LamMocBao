using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Tags")]
    public class Tag : Entity, IEntity
    {
        public string Name { get; set; }
        public string Label { get; set; }
    }

    [Serializable]
    [Table("ProductTags")]
    public class ProductTag : Entity, IEntity
    {
        public Guid ProductId { get; set; }
        public Guid TagId { get; set; }
        public Guid? ProductTypeId { get; set; }


        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }
    }

    [Serializable]
    [Table("ProductTypeTags")]
    public class ProductTypeTag : Entity, IEntity
    {
        public Guid ProductTypeId { get; set; }
        public Guid TagId { get; set; }
        public int SequenceNumber { get; set; }

        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }
    }
}
