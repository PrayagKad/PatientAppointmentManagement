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



    }
}