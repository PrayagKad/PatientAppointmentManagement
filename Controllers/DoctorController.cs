
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using AppointmentBooking.CustomHelpers;
using AppointmentBooking.BLL;
using AppointmentBooking.ViewModels;
using Newtonsoft.Json;
using Serilog;

namespace AppointmentBooking.Controllers
{
    //[RoleAuthorize("Doctor","Admin")]
    public class DoctorController : Controller
    {
        // doctor DashBoard
        public ActionResult DashBoard()
        {
            Log.Information("Doctor dashboard accessed");

            return View();
        }

        //get Patients for a doctor
        public JsonResult GetTodayAppointments()
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            Log.Information("Fetching today's appointments | UserId: {UserId}", userId);

            var appointments = DoctorBLL.GetTodayAppointments(userId);

            Log.Information("Today's appointments fetched successfully | UserId: {UserId}", userId);

            return Json(
                appointments,
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpPost]
        public JsonResult SetSchedule(SetScheduleViewModel sm)
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            Log.Information("Doctor schedule creation attempted | UserId: {UserId}", userId);

            int ret = DoctorBLL.SetDoctorSchedule(userId, sm);

            if (ret > 0)
            {
                Log.Information("Doctor schedule created successfully | UserId: {UserId}", userId);

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
                        Log.Warning("Schedule creation failed | Invalid slot duration | UserId: {UserId}", userId);
                        return Json(new { success = false, message = "Invalid slot duration." });

                    case -2:
                        Log.Warning("Schedule creation failed | Start time must be earlier than end time | UserId: {UserId}", userId);
                        return Json(new { success = false, message = "Start time must be earlier than end time." });

                    case -3:
                        Log.Warning("Schedule creation failed | Doctor not found for user | UserId: {UserId}", userId);
                        return Json(new { success = false, message = "Doctor not found for the specified user." });

                    case -4:
                        Log.Warning("Schedule creation failed | Schedule date is in the past | UserId: {UserId}", userId);
                        return Json(new { success = false, message = "Schedule date cannot be in the past." });

                    case -5:
                        Log.Warning("Schedule creation failed | Schedule already exists | UserId: {UserId}", userId);
                        return Json(new { success = false, message = "A schedule already exists for this doctor on this date." });

                    case -6:
                        Log.Warning("Schedule creation failed | Break start time must be earlier than break end time | UserId: {UserId}", userId);
                        return Json(new { success = false, message = "Break start time must be earlier than break end time." });

                    case -7:
                        Log.Warning("Schedule creation failed | Break outside schedule window | UserId: {UserId}", userId);
                        return Json(new { success = false, message = "Break time must fall within the schedule window." });

                    case -8:
                        Log.Warning("Schedule creation failed | Duplicate breaks detected | UserId: {UserId}", userId);
                        return Json(new { success = false, message = "Duplicate breaks detected." });

                    case -9:
                        Log.Warning("Schedule creation failed | Break times overlap | UserId: {UserId}", userId);
                        return Json(new { success = false, message = "Break times cannot overlap." });

                    default:
                        Log.Error("Schedule creation failed | Unexpected return value: {ReturnValue} | UserId: {UserId}", ret, userId);
                        return Json(new { success = false, message = "An unexpected error occurred." });
                }
            }

        }



        // get schedule dates where a doctor has a schedule 
        [HttpGet]
        public JsonResult GetDoctorScheduleCalendar()
        {
            if (Session["UserId"] == null)
            {
                Log.Warning("Doctor schedule calendar requested without login");

                return Json(new
                {
                    success = false,
                    message = "Please login first."
                }, JsonRequestBehavior.AllowGet);
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            Log.Information("Fetching doctor schedule calendar | UserId: {UserId}", userId);

            DataTable schedules = DoctorBLL.GetDoctorScheduleCalendar(userId);

            Log.Information("Doctor schedule calendar fetched successfully | UserId: {UserId}", userId);

            return Json(JsonConvert.SerializeObject(schedules), JsonRequestBehavior.AllowGet
            );
        }
    }
}