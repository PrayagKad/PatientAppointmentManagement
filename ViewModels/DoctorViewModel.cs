using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentBooking.ViewModels
{
    public class DoctorViewModel
    {
        public int DoctorId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Specialization { get; set; }

        public string Qualification { get; set; }

        public int ExperienceYears { get; set; }
    }
}