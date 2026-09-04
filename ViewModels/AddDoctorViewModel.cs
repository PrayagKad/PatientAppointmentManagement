using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.ViewModels
{
    public class AddDoctorViewModel
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; }

        [StringLength(200)]
        public string Qualification { get; set; }

        public int? ExperienceYears { get; set; }

        [StringLength(1000)]
        public string About { get; set; }

        public decimal? ConsultationFee { get; set; }

        [StringLength(500)]
        public string ClinicAddress { get; set; }

        public string ProfileImage { get; set; }
    }
}