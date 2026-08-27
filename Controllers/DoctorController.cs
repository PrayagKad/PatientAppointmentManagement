using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using AppointmentBooking.CustomHelpers;
using AppointmentBooking.BLL;

namespace AppointmentBooking.Controllers
{
   //[RoleAuthorize("Doctor","Admin")]
    public class DoctorController : Controller
    {
        // doctor DashBoard
        public ActionResult DashBoard()
        {
            return View();
        }

        //get Patients for a doctor
        public JsonResult GetTodayAppointments()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            var appointments =  DoctorBLL.GetTodayAppointments(userId);

            return Json(
                appointments,
                JsonRequestBehavior.AllowGet
            );
        }
    }
}