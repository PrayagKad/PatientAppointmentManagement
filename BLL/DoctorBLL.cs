using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppointmentBooking.DAL;
using AppointmentBooking.ViewModels;
using Serilog;

namespace AppointmentBooking.BLL
{
    public class DoctorBLL
    {
        static string conn = "appointmentDbconnection";

        public static List<AppointmentViewModel> GetTodayAppointments(int userId)
        {
            string sp_name = "sp_GetTodayDoctorAppointments";

            Log.Information("Fetching today's appointments | UserId: {UserId}", userId);

            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd.Add("@UserId", userId);

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name, dd);

            List<AppointmentViewModel> appointments = new List<AppointmentViewModel>();

            if (dt == null)
            {
                Log.Warning("No appointment data returned | UserId: {UserId}", userId);
                return appointments;
            }

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

            Log.Information("Today's appointments fetched | UserId: {UserId} | Count: {Count}", userId, appointments.Count);

            return appointments;
        }


        public static int SetDoctorSchedule(int userId, SetScheduleViewModel sm)
        {
            string sp_name = "sp_CreateDoctorSchedule";

            Log.Information("Creating doctor schedule | UserId: {UserId}", userId);


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
                Log.Warning("Doctor schedule creation failed | No matching schedule dates | UserId: {UserId}", userId);
                return -10;
            }


            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@UserId", userId);
            ds.Add("@StartTime", sm.StartTime);
            ds.Add("@EndTime", sm.EndTime);
            ds.Add("@SlotDurationMinutes", sm.SlotDurationMinutes);



            // add dates
            DataTable dateTable = new DataTable();

            dateTable.Columns.Add("ScheduleDate", typeof(DateTime));

            foreach (DateTime date in dates)
            {
                dateTable.Rows.Add(date.Date);
            }




            // for multiple breaks, created a tvp in sqp and will pass multiple breaks as table

            DataTable breakTable = new DataTable();

            breakTable.Columns.Add("StartTime", typeof(TimeSpan));

            breakTable.Columns.Add("EndTime", typeof(TimeSpan));

            breakTable.Columns.Add("Reason", typeof(string));



            if (sm.Breaks != null)
            {
                foreach (var b in sm.Breaks)
                {
                    breakTable.Rows.Add(b.StartTime, b.EndTime, b.Reason);
                }
            }
            // breaks add in end



            int ret = ExecuteSp.ExecuteNonQueryErrorCode( conn,  sp_name, ds,  dateTable,  breakTable );

            if (ret > 0)
            {
                Log.Information("Doctor schedule created successfully | UserId: {UserId} | Dates: {DateCount} | Breaks: {BreakCount}", userId, dates.Count, breakTable.Rows.Count);
            }
            else if (ret < 0)
            {
                Log.Warning("Doctor schedule creation rejected | UserId: {UserId} | ReturnCode: {ReturnCode}", userId, ret);
            }
            else
            {
                Log.Error("Doctor schedule creation failed unexpectedly | UserId: {UserId} | ReturnCode: {ReturnCode}", userId, ret);
            }

            return ret;
        }


        // get all schedule date a doctor has
        public static DataTable GetDoctorScheduleCalendar(int userId)
        {
            string spName = "sp_GetDoctorScheduleCalendar";


            Log.Information("Fetching doctor schedule calendar | UserId: {UserId}", userId);



            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, spName, parameters);

            if (dt == null)
            {
                Log.Warning("No schedule calendar data returned | UserId: {UserId}", userId);
            }
            else
            {
                Log.Information("Doctor schedule calendar fetched | UserId: {UserId} | Count: {Count}", userId, dt.Rows.Count);
            }

            return dt;
        }
    }
}
