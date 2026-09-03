using System;
using System.Collections.Generic;
using System.Linq;
using AppointmentBooking.ViewModels;
using AppointmentBooking.DAL;
using System.Data;

using System.Web;


namespace AppointmentBooking.BLL
{
    public class PatientBLL
    {
         static string  conn = "appointmentDbconnection";
        public static int RegisterPatient(RegisterViewModel rm)
        {
            string sp_name = "sp_RegisterPatient";

            Dictionary<string, object> pas = new Dictionary<string, object>();

            pas.Add("@FullName", rm.FullName);
            pas.Add("@Email", rm.Email);
            pas.Add("@PasswordHash", rm.Password);
            pas.Add("@Phone", rm.Phone);

            int ret = ExecuteSp.ExecuteNonQuery(conn, sp_name, pas );

            return ret;

        }

        public static DataTable GetDoctors()
        {
            string sp_name = "sp_GetAllDoctors";

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name);

            return dt;

        }

        public static DataTable GetDoctorAvailableDates(int doctorId)
        {
            string sp_name = "sp_GetDoctorAvailableDates";

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@DoctorId", doctorId);

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name, ds);

            return dt;
        }

        public static DataTable GetDoctorAvailableSlots(int doctorId,DateTime selectedDate)
        {

            string sp_name = "sp_GetDoctorAvailableSlots";

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@DoctorId", doctorId);
            ds.Add("@SelectedDate", selectedDate);

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name, ds);
            return dt;
                
        }



    }
}