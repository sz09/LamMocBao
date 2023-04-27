using System;

namespace LamMocBaoWeb.ViewModels
{
    public class SizeViewModel
    {
        public Guid Id { get; set; }

        public double Number { get; set; }
        public string Unit { get; set; }
    }

    public class SizePriceModel
    {
        public Guid SizeId { get; set; }
        public decimal? SellingPrice { get; set; }
    }
}
