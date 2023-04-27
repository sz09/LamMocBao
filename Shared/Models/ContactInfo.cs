using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Shared.Models
{
    [Table("ContactInfos")]
    public class ContactInfo: Entity
    {
        [Required]
        public string PhoneNumber { get; set; }

        public IList<string> ImageUrls { get; set; }
        public string Description { get; set; }
    }
}
