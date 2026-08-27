using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentBooking.ViewModels
{
    public class AppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public string StartTime { get; set; }
        public string PatientName { get; set; }
        public string Status { get; set; }
    }
}