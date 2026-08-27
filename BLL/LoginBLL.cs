using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppointmentBooking.Models;
using AppointmentBooking.DAL;
using System.Data;

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
            DataTable dt = ExecuteSp.ExecuteSelectBy(conn,sp_name,ds);

            if (dt == null || dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            User user = new User
            {
                UserId = Convert.ToInt32(row["UserId"]),
                FullName = row["FullName"].ToString(),
                Email = row["Email"].ToString(),
                Password = row["PasswordHash"].ToString(),
                Role = row["Role"].ToString(),
                isActive = Convert.ToBoolean(row["IsActive"])
            };

            return user;

        }

    }
}