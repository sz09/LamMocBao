using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class UpdateSizeModel
    {
        public Guid Id { get; set; }

        public double Number { get; set; }
        public string Unit { get; set; }
    }
}
