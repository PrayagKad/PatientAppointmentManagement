
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

        [HttpGet]
        public JsonResult GetDoctorAppointments()
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(
                        new
                        {
                            success = false,
                            message = "Please login first."
                        },
                        JsonRequestBehavior.AllowGet
                    );
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                DataTable appointments =
                    DoctorBLL.GetDoctorAppointments(userId);

                Log.Information(
                    "Appointments retrieved. Count: {Count}",
                    appointments.Rows.Count
                );

                var appointmentList = appointments.AsEnumerable()
        .Select(row => new
        {
            AppointmentId = row.Field<int>("AppointmentId"),
            ScheduleDate = row.Field<DateTime>("ScheduleDate").ToString("yyyy-MM-dd"),
            StartTime = row.Field<TimeSpan>("StartTime").ToString(),
            EndTime = row.Field<TimeSpan>("EndTime").ToString(),
            PatientName = row.Field<string>("PatientName"),
            Phone = row.Field<string>("Phone"),
            Status = row.Field<string>("Status")
        })
        .ToList();

                return Json(
                    appointmentList,
                    JsonRequestBehavior.AllowGet
                );
            }
            catch (Exception ex)
            {
                Log.Error(
                    ex,
                    "Error while getting doctor appointments."
                );

                return Json(
                    new
                    {
                        success = false,
                        message = ex.Message
                    },
                    JsonRequestBehavior.AllowGet
                );
            }
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

        [HttpGet]
        public ActionResult GetMedicalRecord(int appointmentId)
        {
            try
            {
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString() != "Doctor")
                {
                    Log.Warning("Unauthorized medical record request | AppointmentId: {AppointmentId}", appointmentId);

                    return new HttpStatusCodeResult(403, "Unauthorized access.");
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                // The appointment must belong to this doctor
                DataRow appointment = DoctorBLL.GetDoctorAppointments(userId)
                    .AsEnumerable()
                    .FirstOrDefault(row => row.Field<int>("AppointmentId") == appointmentId);

                if (appointment == null)
                {
                    Log.Warning("Medical record requested for another doctor's appointment | AppointmentId: {AppointmentId} | UserId: {UserId}", appointmentId, userId);

                    return new HttpStatusCodeResult(403, "Unauthorized access.");
                }

                Log.Information( "Loading medical record for AppointmentId: {AppointmentId}", appointmentId );

                MedicalRecords_ViewModel medicalRecord = DoctorBLL.GetMedicalRecord(appointmentId);

                if (medicalRecord == null)
                {
                    Log.Information( "No medical record found for AppointmentId: {AppointmentId}", appointmentId );

                    medicalRecord = new MedicalRecords_ViewModel
                    {
                        AppointmentId = appointmentId
                    };
                }

                medicalRecord.Status = appointment.Field<string>("Status");

                return PartialView( "_MedicalRecords", medicalRecord  );
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Error loading medical record for AppointmentId: {AppointmentId}",  appointmentId );

                return new HttpStatusCodeResult(  500,"Unable to load medical record." );
            }
        }


        [HttpPost]
        public JsonResult CompleteAppointment(MedicalRecords_ViewModel model)
        {
            try
            {
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString() != "Doctor")
                {
                    Log.Warning("Unauthorized access {UserId}");
                    return Json(new { success = false, message = "Unauthorized access." });
            
                }

                int result = DoctorBLL.CompleteAppointment( model.AppointmentId,  model.Diagnosis,  model.DoctorNotes, model.Medicines, model.Procedures,  model.Advice );

                if (result == 1)
                {
                    Log.Information("Appointment completed successfully. AppointmentId: {AppointmentId}", model.AppointmentId);
                    return Json(new
                    {
                        success = true,
                        message = " Appointment completed successfully"

                    });
                }
                else
                {
                    Log.Warning("Appointment could not be completed. AppointmentId: {AppointmentId}, ResultCode: {ResultCode}", model.AppointmentId, result);

                    return Json(new
                    {
                        success = false,
                        message = " Appointment cannot be completed"

                    });
                }
          

            }
            catch (Exception ex)
            {
                Log.Error(  ex, "Error completing AppointmentId: {AppointmentId}",  model.AppointmentId    );

                return Json(new
                {
                    success = false,
                    message = "Unable to complete appointment."
                });
            }
        }


        // Edit the medical record of an already completed appointment
        [HttpPost]
        public JsonResult UpdateMedicalRecord(MedicalRecords_ViewModel model)
        {
            try
            {
                if (Session["UserId"] == null || Session["Role"] == null || Session["Role"].ToString() != "Doctor")
                {
                    Log.Warning("Unauthorized medical record update | AppointmentId: {AppointmentId}", model.AppointmentId);
                    return Json(new { success = false, message = "Unauthorized access." });
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                int result = DoctorBLL.UpdateMedicalRecord( model.AppointmentId, userId, model.Diagnosis, model.DoctorNotes, model.Medicines, model.Procedures, model.Advice );

                if (result == 1)
                {
                    Log.Information("Medical record updated. AppointmentId: {AppointmentId}", model.AppointmentId);

                    return Json(new { success = true, message = "Medical record updated successfully." });
                }

                Log.Warning("Medical record could not be updated. AppointmentId: {AppointmentId}, ResultCode: {ResultCode}", model.AppointmentId, result);

                return Json(new { success = false, message = "Medical record cannot be updated." });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating medical record. AppointmentId: {AppointmentId}", model.AppointmentId);

                return Json(new { success = false, message = "Unable to update medical record." });
            }
        }

    }
}