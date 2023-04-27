using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Promotions")]
    public class Promotion : Entity, IEntity
    {
        [Column(TypeName = "VARCHAR")]

        [StringLength(20)]
        public string Code { get; set; }
        public string Content { get; set; }
        public double DiscountPercent { get; set; }
        public bool IsActive { get; set; }
        public PromotionMode PromotionMode { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }

    public enum PromotionMode
    {
        Manual = 1,
        Period = 2
    }
}
