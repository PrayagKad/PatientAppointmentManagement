using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

            //?????????????????/

            List<DateTime> dates = new List<DateTime>();
            DateTime currentDate = sm.ScheduleFrom.Date;
            DateTime endDate = sm.ScheduleTo.Date;

            while (currentDate <= endDate)
            {
                string dayName = currentDate.DayOfWeek.ToString();

                if (sm.Days.Any(d =>
                    d.Equals(
                        dayName,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    dates.Add(currentDate);
                }

                currentDate = currentDate.AddDays(1);
            }

            if (dates.Count == 0)
            {
                return -10;
            }

            //????????????????????????????????????????

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@UserId", userId);
           // ds.Add("@ScheduleDate", sm.ScheduleDate);
            ds.Add("@StartTime", sm.StartTime);
            ds.Add("@EndTime", sm.EndTime);
            ds.Add("@SlotDurationMinutes", sm.SlotDurationMinutes);

            // add dates 
            DataTable dateTable = new DataTable();

            dateTable.Columns.Add(
                "ScheduleDate",
                typeof(DateTime));

            foreach (DateTime date in dates)
            {
                dateTable.Rows.Add(date.Date);
            }






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



            int ret = ExecuteSp.ExecuteNonQueryErrorCode(conn, sp_name, ds,dateTable, breakTable);
    

            return ret;
        }
    }
}