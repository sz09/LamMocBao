using LamMocBaoWeb.Models;
using LamMocBaoWeb.Resources;
using System;

namespace LamMocBaoWeb.ViewModels
{
    public class ViewAppointmentViewModel
    {
        public Guid Id { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
        public string InterestedInService { get; set; }
        public DateTime? Birthday { get; set; }
        public BirthDayType BirthDayType { get; set; }
        public string BirthdayStr => Birthday.HasValue ? Birthday.Value.ToString("MM/dd/yyyy HH:mm") : string.Empty;
        public string BirthDayTypeStr => (BirthDayType) switch
        {
            BirthDayType.SolarCalendar => Resource.Customer_Birthday_SolarCalendar,
            BirthDayType.LunarCalendar => Resource.Customer_Birthday_LunarCalendar,
            _ => throw new NotImplementedException()
        };
    }
}
