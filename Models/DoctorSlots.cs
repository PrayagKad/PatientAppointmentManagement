using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentBooking.Models
{
    public class DoctorSlots
    {
        public  int slotId { get; set; }
        public int scheduledId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string status { get; set; }
        public DateTime createdDate { get; set; }
    }
}