using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        [Phone]
        public string Phone { get; set; }
        public string Role { get; set; }
        public  bool isActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }



    }
}