using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AirlinesReservationSystem.Areas.Admin.Controllers
{
    public class BaggageController : Controller
    {
        // GET: Admin/Baggage
        public ActionResult Index()
        {
            return View();
        }
    }
}