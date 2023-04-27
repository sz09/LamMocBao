using System;

namespace LamMocBaoWeb.Models
{
    public class UpdateStockModel
    {
        public Guid Id { get; set; }

        public double Number { get; set; }
        public string Unit { get; set; }
    }
}
