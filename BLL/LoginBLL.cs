
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AppointmentBooking.Models;
using AppointmentBooking.DAL;
using System.Data;
using Serilog;

namespace AppointmentBooking.BLL
{
    public class LoginBLL
    {
        static string conn = "appointmentDbconnection";

        public static User loginCheck(string Email)
        {
            string sp_name = "sp_LoginUser";

            Log.Information("Login user lookup started | Email: {Email}", Email);

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@Email", Email);
            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name, ds);

            if (dt == null || dt.Rows.Count == 0)
            {
                Log.Warning("Login user not found | Email: {Email}", Email);
                return null;
            }

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

            Log.Information("Login user found | UserId: {UserId} | Role: {Role}", user.UserId, user.Role);

            return user;

        }

    }
}

