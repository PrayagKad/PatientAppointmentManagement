using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using AppointmentBooking.BLL;
using Newtonsoft.Json;

namespace AppointmentBooking.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult DashBoard()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetAllDoctors()
        {
            DataTable doctors = PatientBLL.GetDoctors();
            return Json( JsonConvert.SerializeObject(doctors), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDoctorAvailableDates(int doctorId)
        {
            DataTable dates = PatientBLL.GetDoctorAvailableDates(doctorId);

            return Json( JsonConvert.SerializeObject(dates), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDoctorAvailableSlots(int doctorId,DateTime selectedDate)
        {
            DataTable slots = PatientBLL.GetDoctorAvailableSlots(doctorId, selectedDate);

            return Json( JsonConvert.SerializeObject(slots), JsonRequestBehavior.AllowGet );
        }


        public JsonResult GetPatientDetails()
        {

            int UserId = Convert.ToInt32(Session["UserId"]);

            DataTable PatientDetails = PatientBLL.GetPatientDetails(UserId);

            return Json(JsonConvert.SerializeObject(PatientDetails), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ConfirmBooking(  int slotId, string phone,DateTime dateOfBirth, string gender, string address)
        {
            if (Session["UserId"] == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Please login first."
                });
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            int result = PatientBLL.ConfirmBooking(
                userId,
                slotId,
                phone,
                dateOfBirth,
                gender,
                address
            );

            if (result >=  1)
            {
                return Json(new
                {
                    success = true,
                    message = "Appointment booked successfully."
                });
            }

            if (result == -1)
            {
                return Json(new
                {
                    success = false,
                    message = "Patient details not found."
                });
            }

            if (result == -2)
            {
                return Json(new
                {
                    success = false,
                    message = "Sorry, this slot is no longer available."
                });
            }

            return Json(new
            {
                success = false,
                message = "Unable to book appointment."
            });
        }



    }
}