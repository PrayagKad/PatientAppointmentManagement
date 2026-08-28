using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using AppointmentBooking.ViewModels;
using AppointmentBooking.DAL;


namespace AppointmentBooking.BLL
{
    public class AdminBLL
    {
        static string conn = "appointmentDbconnection";
        public static int RegisterDoctor(AddDoctorViewModel ad)
        {
            string sp_name = "sp_CreateDoctor";

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@FullName", ad.FullName);
            ds.Add("@Email", ad.Email);
            ds.Add("@PasswordHash", ad.Password);
            ds.Add("@Phone", ad.Phone);
            ds.Add("@Specialization", ad.Specialization);
            ds.Add("@Qualification", ad.Qualification);
            ds.Add("@ExperienceYears", ad.ExperienceYears);
            ds.Add("@About", ad.About);
            ds.Add("@ConsultationFee", ad.ConsultationFee);
            ds.Add("@ClinicAddress", ad.ClinicAddress);
            ds.Add("@ProfileImage", ad.ProfileImage);

            int ret = ExecuteSp.ExecuteNonQuery(conn,sp_name, ds);

            return ret;
        }
    }
}