using System;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class UpdateMaterialModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
