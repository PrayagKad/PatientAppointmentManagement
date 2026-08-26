using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.Models
{
    public class User
    {
        public int userId { get; set; }
        public string fullName { get; set; }

        [EmailAddress]
        public string email { get; set; }

        public string password { get; set; }

        [Phone]
        public long phone { get; set; }
        public Enum role { get; set; }
        public  bool isActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }



    }
}