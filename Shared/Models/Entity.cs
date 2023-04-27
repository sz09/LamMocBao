using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class Entity: IEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [StringLength(16)]
        public string CreateBy { get; set; }
        public DateTime? ModifiedAt { get; set; } = DateTime.UtcNow;
        [StringLength(16)]
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

    public interface IEntity
    {
        Guid Id { get; set; }
    }

    public interface IIgnoreUTF8NameSearchable
    {
        string NameWithoutUTF8 { get; set; }
        string Name { get; set; }
    }
}
