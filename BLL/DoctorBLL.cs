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

        public static int SetDoctorSchedule(int userId , SetScheduleViewModel sm)
        {
            string sp_name = "sp_CreateDoctorSchedule";

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@UserId", userId);
            ds.Add("@ScheduleDate", sm.ScheduleDate);
            ds.Add("@StartTime", sm.StartTime);
            ds.Add("@EndTime", sm.EndTime);
            ds.Add("@SlotDurationMinutes", sm.SlotDurationMinutes);


            // for multiple breaks , created a tvp in sqp and will pass multiple breaks as table

            DataTable breakTable = new DataTable();

            breakTable.Columns.Add(
                "StartTime",
                typeof(TimeSpan));

            breakTable.Columns.Add(
                "EndTime",
                typeof(TimeSpan));

            breakTable.Columns.Add(
                "Reason",
                typeof(string));


            if (sm.Breaks != null)
            {
                foreach (var b in sm.Breaks)
                {
                    breakTable.Rows.Add(
                        b.StartTime,
                        b.EndTime,
                        b.Reason
                    );
                }
            }

            // break addin end 

            int ret = ExecuteSp.ExecuteNonQueryErrorCode(conn, sp_name, ds, breakTable);
    

            return ret;
        }
    }
}