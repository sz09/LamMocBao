using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("UploadedImages")]
    public class UploadedImage : Entity, IEntity
    {
        public Guid EntityId { get; set; }
        public string Url { get; set; }
        public string UrlPreview { get; set; }
    }
}
