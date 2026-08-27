using System;
using System.Collections.Generic;
using System.Linq;
using AppointmentBooking.ViewModels;
using AppointmentBooking.DAL;

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



    }
}