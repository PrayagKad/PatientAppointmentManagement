using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppointmentBooking.ViewModels;
using AppointmentBooking.BLL;

namespace AppointmentBooking.Controllers
{
    
    public class AdminController : Controller
    {
        public ActionResult ADashBoard()
        {
            return View();
        }

        public ActionResult AddDoctor()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDoctor(AddDoctorViewModel ad)
        {
            if(!ModelState.IsValid)
            {
                return View(ad);
            }

            int ret = AdminBLL.RegisterDoctor(ad);

            if(ret > 0)
            {
                TempData["msg"] = " Doctor registered successfully";
                // add msg in js later that register successful
                return RedirectToAction("ADashBoard", "Admin");
            }

            else
            {
                TempData["msg"] = " doctor not registeres";
                return View(ad);
            } 
        }



        public ActionResult AddAdmin()
        {
            return View();
        }
    }
}