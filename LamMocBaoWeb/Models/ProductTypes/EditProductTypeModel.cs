using Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LamMocBaoWeb.ViewModels
{
    public class EditProductTypeViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập vào tên !")]
        public string Name { get; set; }
        public string LinkName => Name.ToLinkName();
        public int SequenceNumber { get; set; }
        public string DisplayImageUrl { get; set; }
        public string Description { get; set; }
        //public string SubTypes { get; set; }
        public Dictionary<int, Guid> TagIds { get; set; }
        public List<string> ProductTypeTagStrs => TagIds?.Values.Select(d => d.ToString()).ToList() ?? new List<string>();
    }
}
