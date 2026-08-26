using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentBooking.Models
{
    public class Doctor
    {
        public int doctorId{ get; set; }
        public int userId { get; set; }
        public string specialization { get; set; }
        public string qualification { get; set; }
        public int experienceYears { get; set; }
        public string about { get; set; }
        public int fees { get; set; }
        public string clinicAddress { get; set; }
        public string profileImage { get; set; }



    }
}