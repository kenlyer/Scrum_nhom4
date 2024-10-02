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
        public ActionResult View()
        {
            ViewBag.name = "hai";
            return PartialView();
        }
        //Hàm này xử lý yêu cầu tìm kiếm vé máy bay. Nó hiển thị danh sách sân bay xuất phát và đến thông qua ViewBag, sau đó kiểm tra nếu yêu cầu có dữ liệu và ModelState hợp lệ. Nếu hợp lệ, nó kiểm tra các điều kiện về điểm đến và điểm xuất phát, sau đó truy vấn cơ sở dữ liệu để lấy danh sách các chuyến bay phù hợp và trả về chúng dưới dạng View. Nếu không, nó trả về View với _orderTicketForm.
        public ActionResult Index(OrderTicketForm _orderTicketForm)
        {
            ViewBag.from = new SelectList(db.AirPorts, "id", "code");
            ViewBag.to = new SelectList(db.AirPorts, "id", "code");
            ViewBag.flightSchedule = null;
            ViewBag.title = "Search Ticket";
            if(Request.QueryString.Count > 0)
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

                    if(flagError == false)
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

        //Hàm này trả về thông tin chi tiết của một chuyến bay cụ thể dựa trên ID của chuyến bay.
        public ActionResult DetailFlightSchedule(int id)
        {
            FlightSchedule flightSchedule = db.FlightSchedules.Where(s => s.id == id).FirstOrDefault();
            if(flightSchedule == null)
            {
                return HttpNotFound();
            }
            return PartialView(flightSchedule);
        }
  
        //Hàm này hiển thị danh sách vé máy bay của người dùng hiện tại.
        //public ActionResult YourTicket()
        //{
        //    if (!AuthHelper.isLogin())
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    User user = AuthHelper.getIdentity();
        //    IEnumerable<TicketManager> ticketManagers = db.TicketManagers.Where(s => s.user_id == user.id).ToList();
        //    return View(ticketManagers);
        //}
        // Hàm này trả về thông tin chi tiết của một vé máy bay cụ thể dựa trên ID của vé.
        public ActionResult DetailTicket(int id)
        {
            TicketManager ticket = db.TicketManagers.Where(s => s.id == id).FirstOrDefault();
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return PartialView(ticket);
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