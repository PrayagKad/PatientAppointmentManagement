
using System;
using System.Collections.Generic;
using System.Linq;
using AppointmentBooking.ViewModels;
using AppointmentBooking.DAL;
using System.Data;

using System.Web;
using Serilog;


namespace AppointmentBooking.BLL
{
    public class PatientBLL
    {
        static string conn = "appointmentDbconnection";
        public static int RegisterPatient(RegisterViewModel rm)
        {
            string sp_name = "sp_RegisterPatient";

            Log.Information("Patient registration started | Email: {Email}", rm.Email);

            Dictionary<string, object> pas = new Dictionary<string, object>();

            pas.Add("@FullName", rm.FullName);
            pas.Add("@Email", rm.Email);
            pas.Add("@PasswordHash", rm.Password);
            pas.Add("@Phone", rm.Phone);

            int ret = ExecuteSp.ExecuteNonQuery(conn, sp_name, pas);

            if (ret > 0)
            {
                Log.Information("Patient registered successfully | Email: {Email} | Result: {Result}", rm.Email, ret);
            }
            else if (ret == -1)
            {
                Log.Warning("Patient registration failed | Email already registered | Email: {Email}", rm.Email);
            }
            else
            {
                Log.Error("Patient registration failed | Email: {Email} | Result: {Result}", rm.Email, ret);
            }

            return ret;

        }

        public static DataTable GetDoctors()
        {
            string sp_name = "sp_GetAllDoctors";

            Log.Information("Fetching all doctors");

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name);

            Log.Information("Doctors fetched | Count: {Count}", dt?.Rows.Count ?? 0);

            return dt;

        }

        public static DataTable GetDoctorAvailableDates(int doctorId)
        {
            string sp_name = "sp_GetDoctorAvailableDates";

            Log.Information("Fetching doctor available dates | DoctorId: {DoctorId}", doctorId);

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@DoctorId", doctorId);

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name, ds);

            Log.Information("Doctor available dates fetched | DoctorId: {DoctorId} | Count: {Count}", doctorId, dt?.Rows.Count ?? 0);

            return dt;
        }

        public static DataTable GetDoctorAvailableSlots(int doctorId, DateTime selectedDate)
        {

            string sp_name = "sp_GetDoctorAvailableSlots";

            Log.Information("Fetching doctor available slots | DoctorId: {DoctorId} | Date: {SelectedDate}", doctorId, selectedDate);

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@DoctorId", doctorId);
            ds.Add("@SelectedDate", selectedDate);

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name, ds);

            Log.Information("Doctor available slots fetched | DoctorId: {DoctorId} | Date: {SelectedDate} | Count: {Count}", doctorId, selectedDate, dt?.Rows.Count ?? 0);

            return dt;

        }

        public static DataTable GetPatientDetails(int Userid)
        {
            string sp_name = "sp_GetPatientDetails";

            Log.Information("Fetching patient details | UserId: {UserId}", Userid);

            Dictionary<string, object> ds = new Dictionary<string, object>();
            ds.Add("@UserId", Userid);

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, sp_name, ds);

            Log.Information("Patient details fetched | UserId: {UserId} | Count: {Count}", Userid, dt?.Rows.Count ?? 0);

            return dt;





        }

        // confirm booking and also send patient remaining data
        public static int ConfirmBooking(int userId, int slotId, string phone, DateTime dateOfBirth, string gender, string address)
        {
            string spName = "sp_ConfirmBooking";

            Log.Information("Appointment booking started | UserId: {UserId} | SlotId: {SlotId}", userId, slotId);

            Dictionary<string, object> param = new Dictionary<string, object>();

            param.Add("@UserId", userId);
            param.Add("@SlotId", slotId);
            param.Add("@Phone", phone);
            param.Add("@DateOfBirth", dateOfBirth);
            param.Add("@Gender", gender);
            param.Add("@Address", address);

            int ret = ExecuteSp.ExecuteNonQueryErrorCode(conn, spName, param);

            if (ret > 0)
            {
                Log.Information("Appointment booked successfully | UserId: {UserId} | SlotId: {SlotId} | AppointmentId: {AppointmentId}", userId, slotId, ret);
            }
            else if (ret == -1)
            {
                Log.Warning("Appointment booking failed | Patient details not found | UserId: {UserId} | SlotId: {SlotId}", userId, slotId);
            }
            else if (ret == -2)
            {
                Log.Warning("Appointment booking failed | Slot no longer available | UserId: {UserId} | SlotId: {SlotId}", userId, slotId);
            }
            else
            {
                Log.Error("Appointment booking failed | UserId: {UserId} | SlotId: {SlotId} | Result: {Result}", userId, slotId, ret);
            }

            return ret;
        }

        // get appointments or the pateint
        public static DataTable GetPatientAppointments(int userId)
        {
            string spName = "sp_GetPatientAppointments";

            Log.Information("Fetching patient appointments | UserId: {UserId}", userId);

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@UserId", userId);

            DataTable dt = ExecuteSp.ExecuteSelectBy(conn, spName, parameters);

            Log.Information("Patient appointments fetched | UserId: {UserId} | Count: {Count}", userId, dt?.Rows.Count ?? 0);

            return dt;
        }

    }
}
