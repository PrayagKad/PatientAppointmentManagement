using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.Models
{
    public class Patient
    {
        public int patientId { get; set; }
        public int userId { get; set; }

        public DateTime? dateOFbirth { get; set; }

        [StringLength(6)]
        public string gender { get; set; }
        public string address { get; set; }

        [Phone]
        public string phone { get; set; }


    }
}