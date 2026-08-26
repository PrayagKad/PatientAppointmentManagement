using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppointmentBooking.Models;
using AppointmentBooking.DAL;

namespace AppointmentBooking.BLL
{
    public class LoginBLL
    {
        static string conn = "appointmentDbconnection";

        public static User loginCheck(string Email)
        {
            string sp_name = "sp_LoginUser";

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@Email", Email);
            var us = ExecuteSp.ExecuteSelectBy(conn,sp_name,ds);

        }

    }
}