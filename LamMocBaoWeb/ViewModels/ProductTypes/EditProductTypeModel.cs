using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LamMocBaoWeb.Models
{
    public class UpdateProductTypeModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public string LinkName => Name.ToLinkName();
        public int SequenceNumber { get; set; }
        //public string SubTypes { get; set; }
        public Dictionary<int, Guid> TagIds { get; set; }
    }
}
