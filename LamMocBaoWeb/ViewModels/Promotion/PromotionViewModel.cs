using System;

namespace LamMocBaoWeb.ViewModels
{
    public class PromotionViewModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }
        public string Content { get; set; }
        public double DiscountPercent { get; set; }
    }
}
