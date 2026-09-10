
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.Mvc;
using AppointmentBooking.ViewModels;
using AppointmentBooking.BLL;
using AppointmentBooking.Models;
using Serilog;

namespace AppointmentBooking.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Login
        public ActionResult Login()
        {
            // remove cached pages
            Response.Cache.SetCacheability( System.Web.HttpCacheability.NoCache);

            Response.Cache.SetNoStore();

            return View();
        }


        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginViewModel lm)
        {
            Log.Information("Login attempt | Email: {Email}", lm.Email);

            User user = LoginBLL.loginCheck(lm.Email);

            // Invalid credentials
            if (user == null || user.Password != lm.Password)
            {
                Log.Warning("Login failed | Invalid credentials | Email: {Email}", lm.Email  );

                TempData["message"] =  " enter Valid credentials correctly ";

                return View(lm);
            }

            // Session Creation
            Session["UserId"] = user.UserId;
            Session["FullName"] = user.FullName;
            Session["Email"] = user.Email;
            Session["Role"] = user.Role;

            Log.Information(    "Login successful | UserId: {UserId} | Role: {Role}",  user.UserId,  user.Role);

            if (user.Role == "Admin")
            {
                Log.Information( "Redirecting authenticated user | UserId: {UserId} | Destination: Admin Dashboard",  user.UserId  );

                return RedirectToAction( "ADashBoard",  "Admin"  );
            }

            if (user.Role == "Doctor")
            {
                Log.Information( "Redirecting authenticated user | UserId: {UserId} | Destination: Doctor Dashboard",   user.UserId  );

                return RedirectToAction( "DashBoard", "Doctor" );
            }

            if (user.Role == "Patient")
            {
                Log.Information(  "Redirecting authenticated user | UserId: {UserId} | Destination: Patient Dashboard", user.UserId  );

                return RedirectToAction( "Dashboard",  "Patient"   );
            }

            // Unexpected role
            Log.Error("Login failed | Invalid user role | UserId: {UserId} | Role: {Role}",user.UserId, user.Role );

            ModelState.AddModelError( "", "Invalid user role." );

            return View(lm);
        }


        // GET: Account/RegisterPatient
        public ActionResult RegisterPatient()
        {
            return View();
        }


        // POST: Account/RegisterPatient
        [HttpPost]
        public ActionResult RegisterPatient(RegisterViewModel rm)
        {
            Log.Information("Patient registration attempt | Email: {Email}", rm.Email );

            if (!ModelState.IsValid)
            {
                Log.Warning( "Patient registration failed validation | Email: {Email}", rm.Email );

                return View(rm);
            }

            int rt = PatientBLL.RegisterPatient(rm);

            if (rt > 0)
            {
                Log.Information(
                    "Patient registered successfully | PatientId: {PatientId} | Email: {Email}",  rt,  rm.Email
                );

                TempData["message"] = "patient registered successfully";

                return RedirectToAction( "Login", "Account" );
            }

            if (rt == -1)
            {
                Log.Warning( "Patient registration failed | Email already registered | Email: {Email}", rm.Email  );

                TempData["message"] = "Email Already Registered";

                return View(rm);
            }

            Log.Error(  "Patient registration failed | BLL returned unexpected result | Result: {Result} | Email: {Email}",  rt,  rm.Email );

            return View(rm);
        }


        // Check if patient has logged in
        [HttpGet]
        public JsonResult IsLoggedIn()
        {
            bool isLoggedIn = Session["UserId"] != null;

            Log.Information( "Login status checked | IsLoggedIn: {IsLoggedIn}", isLoggedIn );

            return Json(new { 
                isLoggedIn = isLoggedIn 
            }, JsonRequestBehavior.AllowGet
            );
        }
    }
}

