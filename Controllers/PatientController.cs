using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using AppointmentBooking.BLL;
using AppointmentBooking.ViewModels;
using Newtonsoft.Json;
using Serilog;

namespace AppointmentBooking.Controllers
{
    public class PatientController : Controller
    {
        // Doctor list shown on the patient dashboard; cleared when a doctor is added
        public const string DoctorsCacheKey = "Patient_AllDoctors";

        private static readonly TimeSpan DoctorsCacheDuration = TimeSpan.FromMinutes(10);


        // GET: Patient
        public ActionResult DashBoard()
        {
            Log.Information("Patient dashboard accessed");

            return View();
        }


        [HttpGet]
        public JsonResult GetAllDoctors()
        {
            string doctorsJson = HttpRuntime.Cache[DoctorsCacheKey] as string;

            if (doctorsJson != null)
            {
                Log.Information("All doctors served from cache");

                return Json(doctorsJson, JsonRequestBehavior.AllowGet);
            }

            Log.Information("Fetching all doctors");

            DataTable doctors = PatientBLL.GetDoctors();

            Log.Information("All doctors fetched | Count: {Count}", doctors?.Rows.Count ?? 0);

            doctorsJson = JsonConvert.SerializeObject(doctors);

            HttpRuntime.Cache.Insert(
                DoctorsCacheKey,
                doctorsJson,
                null,
                DateTime.UtcNow.Add(DoctorsCacheDuration),
                Cache.NoSlidingExpiration);

            return Json(doctorsJson, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetDoctorAvailableDates(int doctorId)
        {
            Log.Information("Fetching available dates | DoctorId: {DoctorId}", doctorId);

            DataTable dates = PatientBLL.GetDoctorAvailableDates(doctorId);

            Log.Information("Available dates fetched | DoctorId: {DoctorId} | Count: {Count}", doctorId, dates?.Rows.Count ?? 0);

            return Json(JsonConvert.SerializeObject(dates), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetDoctorAvailableSlots(int doctorId, DateTime selectedDate)
        {
            Log.Information("Fetching available slots | DoctorId: {DoctorId} | Date: {SelectedDate}", doctorId, selectedDate);

            DataTable slots = PatientBLL.GetDoctorAvailableSlots(doctorId, selectedDate);

            Log.Information("Available slots fetched | DoctorId: {DoctorId} | Date: {SelectedDate} | Count: {Count}", doctorId, selectedDate, slots?.Rows.Count ?? 0);

            return Json(JsonConvert.SerializeObject(slots), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPatientDetails()
        {
            int UserId = Convert.ToInt32(Session["UserId"]);

            Log.Information("Fetching patient details | UserId: {UserId}", UserId);

            DataTable PatientDetails = PatientBLL.GetPatientDetails(UserId);

            Log.Information("Patient details fetched | UserId: {UserId}", UserId);

            return Json(JsonConvert.SerializeObject(PatientDetails), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ConfirmBooking(int slotId, string phone, DateTime dateOfBirth, string gender, string address)
        {
            if (Session["UserId"] == null)
            {
                Log.Warning("Appointment booking attempted without login | SlotId: {SlotId}", slotId);

                return Json(new
                {
                    success = false,
                    message = "Please login first."
                });
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            Log.Information("Appointment booking attempted | UserId: {UserId} | SlotId: {SlotId}", userId, slotId);

            int result = PatientBLL.ConfirmBooking(userId, slotId, phone, dateOfBirth, gender, address);

            if (result >= 1)
            {
                Log.Information("Appointment booked successfully | UserId: {UserId} | SlotId: {SlotId} | AppointmentId: {AppointmentId}", userId, slotId, result);

                return Json(new
                {
                    success = true,
                    message = "Appointment booked successfully."
                });
            }

            if (result == -1)
            {
                Log.Warning("Appointment booking failed | Patient details not found | UserId: {UserId} | SlotId: {SlotId}", userId, slotId);

                return Json(new
                {
                    success = false,
                    message = "Patient details not found."
                });
            }

            if (result == -2)
            {
                Log.Warning("Appointment booking failed | Slot no longer available | UserId: {UserId} | SlotId: {SlotId}", userId, slotId);

                return Json(new
                {
                    success = false,
                    message = "Sorry, this slot is no longer available."
                });
            }

            Log.Error("Appointment booking failed unexpectedly | UserId: {UserId} | SlotId: {SlotId} | ReturnCode: {ReturnCode}", userId, slotId, result);

            return Json(new
            {
                success = false,
                message = "Unable to book appointment."
            });
        }


        [HttpGet]
        public JsonResult GetPatientAppointments()
        {
            if (Session["UserId"] == null)
            {
                Log.Warning("Patient appointments requested without login");

                return Json(new
                {
                    success = false,
                    message = "Please login first."
                }, JsonRequestBehavior.AllowGet);
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            Log.Information("Fetching patient appointments | UserId: {UserId}", userId);

            DataTable appointments = PatientBLL.GetPatientAppointments(userId);

            Log.Information("Patient appointments fetched | UserId: {UserId} | Count: {Count}", userId, appointments?.Rows.Count ?? 0);

            return Json(JsonConvert.SerializeObject(appointments), JsonRequestBehavior.AllowGet);
        }


        // Read-only medical record of one of the patient's own completed appointments
        [HttpGet]
        public ActionResult GetMedicalRecord(int appointmentId)
        {
            if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString() != "Patient")
            {
                Log.Warning("Medical record requested without patient login | AppointmentId: {AppointmentId}", appointmentId);

                return new HttpStatusCodeResult(403, "Please login first.");
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            try
            {
                // The appointment must belong to this patient and be completed
                DataRow appointment = PatientBLL.GetPatientAppointments(userId)
                    .AsEnumerable()
                    .FirstOrDefault(row => row.Field<int>("AppointmentId") == appointmentId);

                if (appointment == null)
                {
                    Log.Warning("Medical record requested for another patient's appointment | AppointmentId: {AppointmentId} | UserId: {UserId}", appointmentId, userId);

                    return new HttpStatusCodeResult(403, "Unauthorized access.");
                }

                if (appointment.Field<string>("Status") != "Completed")
                {
                    return new HttpStatusCodeResult(400, "Medical record is available after the appointment is completed.");
                }

                Log.Information("Patient viewing medical record | AppointmentId: {AppointmentId} | UserId: {UserId}", appointmentId, userId);

                MedicalRecords_ViewModel medicalRecord = DoctorBLL.GetMedicalRecord(appointmentId)
                    ?? new MedicalRecords_ViewModel { AppointmentId = appointmentId };

                medicalRecord.Status = "Completed";

                return PartialView("_MedicalRecords", medicalRecord);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading patient medical record | AppointmentId: {AppointmentId} | UserId: {UserId}", appointmentId, userId);

                return new HttpStatusCodeResult(500, "Unable to load medical record.");
            }
        }

        //[HttpPost]
        //public JsonResult CancelAppointment(int appointmentId)
        //{
        //    try
        //    {
        //        if (Session["UserId"] == null ||
        //            Session["Role"] == null ||
        //            Session["Role"].ToString() != "Patient")
        //        {
        //            return Json(new
        //            {
        //                success = false,
        //                message = "Unauthorized."
        //            });
        //        }

        //        int userId = Convert.ToInt32(Session["UserId"]);

        //        //int result = PatientBLL.CancelAppointment(
        //        //    appointmentId,
        //        //    userId
        //        //);

        //        if (result == 1)
        //        {
        //            Log.Information(
        //                "Appointment cancelled successfully. AppointmentId: {AppointmentId}",
        //                appointmentId
        //            );

        //            return Json(new
        //            {
        //                success = true,
        //                message = "Appointment cancelled successfully."
        //            });
        //        }
        //        else
        //        {
        //            Log.Warning(
        //                "Appointment could not be cancelled. AppointmentId: {AppointmentId}, ResultCode: {ResultCode}",
        //                appointmentId,
        //                result
        //            );

        //            return Json(new
        //            {
        //                success = false,
        //                message = "Appointment cannot be cancelled."
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(
        //            ex,
        //            "Error cancelling AppointmentId: {AppointmentId}",
        //            appointmentId
        //        );

        //        return Json(new
        //        {
        //            success = false,
        //            message = "Unable to cancel appointment."
        //        });
        //    }
        //}
    }
}

