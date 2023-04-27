using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("CustomerDesirings")]
    public class CustomerDesiring: Entity, IEntity
    {
        [Required]
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime? Birthday{ get; set; }
        public virtual ICollection<CustomerPreferedInfos> CustomerPrefereds { get; set; }

    }

    [Serializable]
    [Table("CustomerPreferedInfos")]
    public class CustomerPreferedInfos : Entity, IEntity
    {
        public Guid CustomerDesiringId { get; set; }
        public Guid PreferedProductId { get; set; }
        [ForeignKey("PreferedProductId")]
        public virtual Product PreferedProduct { get; set; }
        [ForeignKey("CustomerDesiringId")]
        public virtual CustomerDesiring CustomerDesiring { get; set; }
    }
}
