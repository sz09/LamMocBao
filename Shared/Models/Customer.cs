using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Customers")]
    public class Customer : Entity, IEntity
    {
        [StringLength(200)]
        public string FullName { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        public BirthDayType BirthDayType { get; set; }
        [StringLength(200)]
        public string Province { get; set; }
        [StringLength(200)]
        public string District { get; set; }
        [StringLength(200)]
        public string Ward { get; set; }
        public string Address { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public enum BirthDayType
    {
        SolarCalendar = 1,
        LunarCalendar = 2,
    }
}
