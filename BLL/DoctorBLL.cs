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

        public static DataTable GetDoctorAppointments(int userId)
        {
            string spName = "sp_GetDoctorAppointments";

            try
            {
                Log.Information(
                    "Getting appointments for doctor UserId: {UserId}",
                    userId
                );

                Dictionary<string, object> para =new Dictionary<string, object>();

                para.Add("@UserId", userId);

                DataTable result = ExecuteSp.ExecuteSelectBy( conn,spName, para  );

                Log.Information( "Successfully retrieved appointments for doctor UserId: {UserId}. Count: {Count}", userId,  result.Rows.Count);

                return result;
            }
            catch (Exception ex)
            {
                Log.Error( ex,  "Error while getting appointments for doctor UserId: {UserId}",   userId );

                throw;
            }
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

  

            public static MedicalRecords_ViewModel GetMedicalRecord(int appointmentId)
            {
                string spName = "sp_GetMedicalRecord";

                try
                {
                    Log.Information("Getting medical record for AppointmentId: {AppointmentId}", appointmentId  );

                    Dictionary<string, object> parameters =  new Dictionary<string, object>();

                    parameters.Add("@AppointmentId", appointmentId);

                    DataTable dt = ExecuteSp.ExecuteSelectBy(conn,spName, parameters);

                    if (dt.Rows.Count == 0)
                    {
                        Log.Information( "No medical record found for AppointmentId: {AppointmentId}", appointmentId );

                        return null;
                    }

                    DataRow row = dt.Rows[0];

                    MedicalRecords_ViewModel record =  new MedicalRecords_ViewModel
                        {
                            MedicalRecordId = row.Field<int?>("MedicalRecordId") ?? 0,

                            AppointmentId = row.Field<int>("AppointmentId"),

                            PatientName = row.Field<string>("PatientName"),

                            DoctorName = row.Field<string>("DoctorName"),

                            AppointmentDate = row.Field<DateTime>("AppointmentDate"),

                            Time = row.Field<string>("Time"),

                            Diagnosis = row.Field<string>("Diagnosis"),

                            DoctorNotes = row.Field<string>("DoctorNotes"),

                            Medicines = row.Field<string>("Medicines"),

                            Procedures = row.Field<string>("Procedures"),

                            Advice = row.Field<string>("Advice")
                        };

                Log.Information( "Medical record retrieved successfully for AppointmentId: {AppointmentId}", appointmentId );

                    return record;
                }
                catch (Exception ex)
                {
                    Log.Error( ex,"Error getting medical record for AppointmentId: {AppointmentId}",  appointmentId  );

                    throw;
                }
            }

        public static int CompleteAppointment(int appointmentId, string diagnosis, string doctorNotes, string medicines, string procedures, string advice)
        {
            string spName = "sp_CompleteAppointment";

            try
            {
                Log.Information("Completing AppointmentId: {AppointmentId}", appointmentId);

                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add("@AppointmentId", appointmentId);
                parameters.Add("@Diagnosis", diagnosis);
                parameters.Add("@DoctorNotes", doctorNotes);
                parameters.Add("@Medicines", medicines);
                parameters.Add("@Procedures", procedures);
                parameters.Add("@Advice", advice);

                int result = ExecuteSp.ExecuteNonQueryErrorCode( conn, spName,  parameters );

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error completing AppointmentId: {AppointmentId}", appointmentId);
                throw;
            }
        }
    }
    }
    

