using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    [Serializable]
    [Table("Appointments")]
    public class Appointment : Entity, IEntity
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string InterestedInService { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
        public DateTime? BirthDay { get; set; }
        public BirthDayType BirthDayType { get; set; }
    }
}
