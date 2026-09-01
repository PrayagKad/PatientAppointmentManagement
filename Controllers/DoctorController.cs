using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using AppointmentBooking.CustomHelpers;
using AppointmentBooking.BLL;
using AppointmentBooking.ViewModels;

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

        [HttpPost]
        public JsonResult SetSchedule(SetScheduleViewModel sm)
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            int ret = DoctorBLL.SetDoctorSchedule(userId, sm);

            if (ret > 0)
            {
                return Json(new
                {
                    success = true,
                    message = "Schedule created successfully."
                });
            }

            else
            {
                switch (ret)
                {
                    case -1:
                        return Json(new { success = false, message = "Invalid slot duration." });

                    case -2:
                        return Json(new { success = false, message = "Start time must be earlier than end time." });

                    case -3:
                        return Json(new { success = false, message = "Doctor not found for the specified user." });

                    case -4:
                        return Json(new { success = false, message = "Schedule date cannot be in the past." });

                    case -5:
                        return Json(new { success = false, message = "A schedule already exists for this doctor on this date." });

                    case -6:
                        return Json(new { success = false, message = "Break start time must be earlier than break end time." });

                    case -7:
                        return Json(new { success = false, message = "Break time must fall within the schedule window." });

                    case -8:
                        return Json(new { success = false, message = "Duplicate breaks detected." });

                    case -9:
                        return Json(new { success = false, message = "Break times cannot overlap." });

                    default:
                        return Json(new { success = false, message = "An unexpected error occurred." });
                }
            }

        }
    }
}