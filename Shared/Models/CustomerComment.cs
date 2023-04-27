using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("CustomerComments")]
    public class CustomerComment : Entity, IEntity
    {
        public string Hint { get; set; }
        public Guid UploadedImageId { get; set; }
        public int SequenceNumber { get; set; }

        public virtual UploadedImage UploadedImage { get; set; }
    }
}
