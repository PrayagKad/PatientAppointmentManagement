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

            // remove cached pages 
            Response.Cache.SetCacheability( System.Web.HttpCacheability.NoCache);

            Response.Cache.SetNoStore();

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

            // Session Creation

            Session["UserId"] = user.UserId;
            Session["FullName"] = user.FullName;
            Session["Email"] = user.Email;
            Session["Role"] = user.Role;

            //// states the user is logged in or not for other requests
            //FormsAuthentication.SetAuthCookie(
            //     user.UserId.ToString(),
            //     lm.RememberMe);

            if (user.Role == "Admin")
            {
                return RedirectToAction("ADashBoard", "Admin");
            }

            if (user.Role == "Doctor")
            {
                return RedirectToAction("DashBoard", "Doctor");
            }

            if (user.Role == "Patient")
            {
                return RedirectToAction("Dashboard", "Patient");
            }

           //  FormsAuthentication.SignOut();

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


        
        // check if patient has logged in 
        [HttpGet]
        public JsonResult IsLoggedIn()
        {
            bool isLoggedIn = Session["UserId"] != null;

            return Json(
                new { isLoggedIn = isLoggedIn },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}