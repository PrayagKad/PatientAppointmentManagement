using System;
using System.Collections.Generic;
using System.Data;
using AppointmentBooking.DAL;
using AppointmentBooking.ViewModels;

namespace AppointmentBooking.BLL
{
    public class DoctorBLL
    {
        static string conn = "appointmentDbconnection";
        public static List<AppointmentViewModel> GetTodayAppointments(int userId)
        {

            string sp_name = "sp_GetTodayDoctorAppointments";
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd.Add("@UserId", userId);

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name, dd);

            List<AppointmentViewModel> appointments =  new List<AppointmentViewModel>();

            if (dt == null)
                return appointments;

            foreach (DataRow row in dt.Rows)
            {
                appointments.Add(new AppointmentViewModel
                {
                    AppointmentId = Convert.ToInt32(row["AppointmentId"]),
                    StartTime = row["StartTime"].ToString(),
                    PatientName = row["PatientName"].ToString(),
                    Status = row["Status"].ToString()
                });
            }

            return appointments;



        }
    }
}