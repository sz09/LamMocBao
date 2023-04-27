using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Analysis")]
    public class Analysis : Entity, IEntity
    {
        public Guid EntityId { get; set; }
        public int AccessCount { get; set; }
        public EntityType EntityType { get; set; }
    }
}
