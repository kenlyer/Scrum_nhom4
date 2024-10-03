using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AirlinesReservationSystem.Models.Form;
using AirlinesReservationSystem.Models;
using AirlinesReservationSystem.Helper;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Net;

namespace AirlinesReservationSystem.Controllers
{
    public class HomeController : Controller
    {
        private Model1 db = new Model1();

        [HttpGet]
        //[ValidateAntiForgeryToken]

        //Hàm này trả về một PartialView và gán giá trị "hai" cho biến ViewBag.name.
        public ActionResult Index(OrderTicketForm _orderTicketForm)
        {
            ViewBag.from = new SelectList(db.AirPorts, "id", "code");
            ViewBag.to = new SelectList(db.AirPorts, "id", "code");
            ViewBag.flightSchedule = null;
            ViewBag.title = "Search Ticket";
            if (Request.QueryString.Count > 0)
            {
                if (ModelState.IsValid)
                {
                    // Đây là cờ báo lỗi
                    bool flagError = false;
                    // Điểm đến trùng điểm đi
                    if (!_orderTicketForm.checkDestination())
                    {
                        ModelState.AddModelError("to", "The destination must be different from the point of departure");
                        flagError = true;
                    }

                    if (flagError == false)
                    {
                        //Lấy danh sach chuyến bay phù hợp vs thời gian.

                        var query = db.FlightSchedules.Where(s => s.to_airport == _orderTicketForm.to && s.from_airport == _orderTicketForm.from);
                        DateTime repartureDate = DateTime.Parse(_orderTicketForm.repartureDate);
                        query = query.Where(s => EntityFunctions.TruncateTime(s.departures_at) == EntityFunctions.TruncateTime(repartureDate));
                        List<FlightSchedule> models = query.ToList();
                        ViewBag.flightSchedule = models;
                        return View(_orderTicketForm);
                    }

                }
            }
            else
            {
                ModelState.Clear();
            }
            return View(_orderTicketForm);
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Deals()
        {
            return View();
        }
    }
}