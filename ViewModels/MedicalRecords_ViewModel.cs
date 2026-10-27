using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppointmentBooking.ViewModels
{
    public class MedicalRecords_ViewModel
    {
        public int MedicalRecordId { get; set; }

        public int AppointmentId { get; set; }

        public string PatientName { get; set; }

        public string DoctorName { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string Time { get; set; }

        public string Diagnosis { get; set; }

        public string DoctorNotes { get; set; }

        public string Medicines { get; set; }

        public string Procedures { get; set; }

        public string Advice { get; set; }
    }
}