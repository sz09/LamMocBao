using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("HighlightItems")]
    public class HighlightItem : Entity, IEntity
    {
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    public enum EntityType
    {
        [Description("Sản phẩm")]
        Products = 1,
        [Description("Kiến thức")]
        Knowledges = 2,
    }
}
