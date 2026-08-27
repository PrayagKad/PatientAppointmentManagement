using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.Mvc;
using AppointmentBooking.ViewModels;
using AppointmentBooking.BLL;
using AppointmentBooking.Models;

namespace AppointmentBooking.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel lm)
        {

            User user = LoginBLL.loginCheck(lm.Email);

            if(user == null || user.Password != lm.Password)
            {
                TempData["message"] = " enter Valid credentials correctly ";
                return View(lm);
            }

            // states the user is authenticated or not for other requests
                 FormsAuthentication.SetAuthCookie(
                 user.UserId.ToString(),
                 lm.RememberMe);

            if (user.Role == "Admin")
            {
                return RedirectToAction("DashBoard", "Admin");
            }

            if (user.Role == "Doctor")
            {
                return RedirectToAction("DashBoard", "Doctor");
            }

            if (user.Role == "Patient")
            {
                return RedirectToAction("Home", "Patient");
            }

            FormsAuthentication.SignOut();

            ModelState.AddModelError(
                "",
                "Invalid user role."
            );

            return View(lm);

           

        }

        public ActionResult RegisterPatient()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterPatient(RegisterViewModel rm)
        {
            if(!ModelState.IsValid)
            {
                return View(rm);
            }
            int rt = PatientBLL.RegisterPatient(rm);

            if(rt > 0)
            {
                TempData["message"] = "patient registered successfully";
                return RedirectToAction("Login", "Account");
            }
            if(rt == -1)
            {
                TempData["message"] = "Email Already Registered";
                return View(rm);
            }
            return View(rm);
           
        }
    }
}