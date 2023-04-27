using Shared.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Knowledges")]
    public class Knowledge : Entity, IEntity, IIgnoreUTF8NameSearchable
    {
        [StringLength(200), Required]
        public string Name { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(200), Required]
        public string NameWithoutUTF8 { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(200), Required]
        public string LinkName { get; set; }
        [StringLength(200)]
        public string Summary { get; set; }
        public string Content { get; set; }
        public Guid UploadedImageId { get; set; }
        public int SequenceNumber { get; set; }
        [ForeignKey("UploadedImageId")]
        public virtual UploadedImage UploadedImage { get; set; }
    }

    [Serializable]
    [Table("PublishedKnowledges")]
    public class PublishedKnowledge : Entity, IEntity, IIgnoreUTF8NameSearchable
    {
        public Guid OriginKnowledgeId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(200), Required]
        public string NameWithoutUTF8 { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(200)]
        public string LinkName { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public int SequenceNumber { get; set; }
        public Guid UploadedImageId { get; set; }
        [ForeignKey("UploadedImageId")]
        public virtual UploadedImage UploadedImage { get; set; }
    }
}
