namespace LamMocBaoWeb.Models.Appointment
{
    public class AppointmentModel
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string NumberAndStreet { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string BirthDayDate { get; set; }
        public string BirthDayTime { get; set; }
        public BirthDayType BirthDayType { get; set; }
        public string Note { get; set; }
        public string InterestedInService { get; set; }
    }
}
