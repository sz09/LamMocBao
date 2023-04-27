using LamMocBaoWeb.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class PromotionModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào mã code !")]
        public string Code { get; set; }
        public string Content { get; set; }
        public double DiscountPercent { get; set; }
        public bool IsActive { get; set; }
        public PromotionMode PromotionMode { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
