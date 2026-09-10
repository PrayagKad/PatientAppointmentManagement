using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using AppointmentBooking.BLL;
using Newtonsoft.Json;
using Serilog;

namespace AppointmentBooking.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult DashBoard()
        {
            Log.Information("Patient dashboard accessed");

            return View();
        }


        [HttpGet]
        public JsonResult GetAllDoctors()
        {
            Log.Information("Fetching all doctors");

            DataTable doctors = PatientBLL.GetDoctors();

            Log.Information("All doctors fetched | Count: {Count}", doctors?.Rows.Count ?? 0);

            return Json(JsonConvert.SerializeObject(doctors), JsonRequestBehavior.AllowGet);
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
    }
}

