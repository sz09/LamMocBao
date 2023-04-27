using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("NewsPaperPosts")]
    public class NewsPaperPost : Entity, IEntity
    {
        public string Hint { get; set; }
        public string Link { get; set; }
        public Guid UploadedImageId { get; set; }
        public int SequenceNumber { get; set; }

        public virtual UploadedImage UploadedImage { get; set; }
    }
}
