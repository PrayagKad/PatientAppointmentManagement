using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppointmentBooking.Controllers
{
    [Authorize(Roles ="Doctor")]
    [Authorize(Roles ="Admin")]
    public class DoctorController : Controller
    {
        // GET: Doctor
        public ActionResult Index()
        {
            return View();
        }
    }
}