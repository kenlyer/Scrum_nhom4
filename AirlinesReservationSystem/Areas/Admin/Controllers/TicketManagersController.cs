using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirlinesReservationSystem.Helper;
using AirlinesReservationSystem.Models;

namespace AirlinesReservationSystem.Areas.Admin.Controllers
{
    public class TicketManagersController : Controller
    {
        private Model1 db = new Model1();
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (AuthHelper.getIdentityEmployeee() == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Controller = "Security", Action = "Login" }));
            }
            base.OnActionExecuted(filterContext);
        }

        public ActionResult Author()
        {
            if (AuthHelper.isLoginEmployeee() == false)
            {
                return RedirectToAction("Login", "Security");
            }
            else return null;
        }


        // GET: Admin/TicketManagers
        public ActionResult Index()
        {
            Author();
            var ticketManagers = db.TicketManagers.Include(t => t.FlightSchedule).Include(t => t.User).OrderByDescending(x => x.id);
            return View(ticketManagers.ToList());
        }

        // GET: Admin/TicketManagers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketManager ticketManager = db.TicketManagers.Find(id);
            if (ticketManager == null)
            {
                return HttpNotFound();
            }
            return View(ticketManager);
        }

        // GET: Admin/TicketManagers/Create
        public ActionResult Create()
        {
            ViewBag.flight_schedules_id = new SelectList(db.FlightSchedules, "id", "code");
            ViewBag.user_id = new SelectList(
               db.Users.Select(u => new
               {
                   id = u.id,
                   name_phone = u.name + " - " + u.phone_number
               }),
               "id",
               "name_phone"
           );
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Pay" },
                new SelectListItem { Value = "1", Text = "Cancel" }
            };
            ViewBag.pay_id = new SelectList(db.Payments.Select(u => new { id = u.id, name = u.id + "-" + u.name_Payment + "-" + u.PayerID_Payment }), "id", "name");
            ViewBag.status = new SelectList(statusList, "Value", "Text");
            return View();
        }

        // POST: Admin/TicketManagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,flight_schedules_id,user_id,status,seat_location,pay_id")] TicketManager ticketManager)
        {
            FlightSchedule a = db.FlightSchedules.Find(ticketManager.flight_schedules_id);
            //kiểm tra vị trí 
            Seats seat = db.Seats.FirstOrDefault(x => x.flight_schedules_id == a.id && x.seat == ticketManager.seat_location.ToString() && x.isbooked == 1);
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Pay" },
                new SelectListItem { Value = "1", Text = "Cancel" }
            };
            ViewBag.pay_id = new SelectList(db.Payments.Select(u => new { id = u.id, name = u.id + "-" + u.name_Payment + "-" + u.PayerID_Payment }), "id", "name");
            ViewBag.status = new SelectList(statusList, "Value", "Text", ticketManager.status);
            ViewBag.flight_schedules_id = new SelectList(db.FlightSchedules, "id", "id", ticketManager.flight_schedules_id);
            ViewBag.user_id = new SelectList(db.Users, "id", "name", ticketManager.user_id);
            if (seat != null)
            {
                ModelState.AddModelError("seat_location", "chỗ ngồi đã có người đặt rồi hoặc đang đặt.");
                return View(ticketManager);
            }
            if (ModelState.IsValid)
            {
                ticketManager.code = a.code;
                db.TicketManagers.Add(ticketManager);
                db.SaveChanges();
                AlertHelper.setAlert("success", "Tạo dữ liệu vé bay thành công.");
                IncreaseSeats(ticketManager.flight_schedules_id);
                ChangeSeats(ticketManager.flight_schedules_id, ticketManager.seat_location.ToString(), 0);
                return RedirectToAction("Index");
            }


            return View(ticketManager);
        }



        [HttpGet]
        public JsonResult GetUserOptions(String searchValue)
        {
            var users = db.Users
           .Select(u => new { u.id, u.name, u.phone_number })
           .ToList();
            // Lấy danh sách người dùng từ cơ sở dữ liệu
            if (searchValue.Trim() == "" || searchValue == null)
            {
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            users = db.Users
              .Select(u => new { u.id, u.name, u.phone_number })
              .Where(u => u.name.Contains(searchValue.Trim()) || u.phone_number.Contains(searchValue.Trim()))
              .ToList();
            // Trả về dữ liệu dưới dạng JSON
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetUserOptionsPay(String searchValue)
        {
            var payment = db.Payments.Select(u => new { u.id, u.name_Payment, u.PayerID_Payment }).ToList();
            // Lấy danh sách người dùng từ cơ sở dữ liệu
            if (searchValue != null)
            {
                payment = db.Payments.Select(u => new { u.id, u.name_Payment, u.PayerID_Payment }).Where(u => u.name_Payment.Contains(searchValue.Trim()) || u.PayerID_Payment.Contains(searchValue.Trim())).ToList();
                return Json(payment, JsonRequestBehavior.AllowGet);
            }
            else
            {
                payment = db.Payments.Select(u => new { u.id, u.name_Payment, u.PayerID_Payment }).ToList();
                return Json(payment, JsonRequestBehavior.AllowGet);
            }
            // Trả về dữ liệu dưới dạng JSON

        }






        // GET: Admin/TicketManagers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketManager ticketManager = db.TicketManagers.Find(id);
            if (ticketManager == null)
            {
                return HttpNotFound();
            }
            if (ticketManager.pay_id != null)
            {
                ViewBag.pay_id = new SelectList(db.Payments.Select(u => new { id = u.id, name = u.id + "-" + u.name_Payment + "-" + u.PayerID_Payment }), "id", "name", ticketManager.pay_id);
            }
            else
            {
                ViewBag.pay_id = new SelectList(db.Payments.Where(x => x.PayerID_Payment == ticketManager.pay_id.ToString()).Select(u => new { id = u.id, name = u.id + "-" + u.name_Payment + "-" + u.PayerID_Payment }), "id", "name", ticketManager.pay_id);
            }
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Pay" },
                new SelectListItem { Value = "1", Text = "Cancel" }
            };
            ViewBag.user_id = new SelectList(
                db.Users.Select(u => new
                {
                    id = u.id,
                    name_phone = u.name + " - " + u.phone_number
                }),
                "id",
                "name_phone",
                ticketManager.user_id
            );
            ViewBag.flight_schedules_id = new SelectList(db.FlightSchedules, "id", "code", ticketManager.flight_schedules_id);
            ViewBag.status = new SelectList(statusList, "Value", "Text", ticketManager.status);


            //ViewBag.user_id = db.Users.Select(r => new SelectListItem() { Value = r.id.ToString(), Text = r.name + " " + r.phone_number  });
            //ViewBag.DefaultUser = db.Users.Find(ticketManager.user_id);


            return View(ticketManager);
        }

        // POST: Admin/TicketManagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,flight_schedules_id,user_id,status,code,seat_location,pay_id")] TicketManager ticketManager)
        {

            if (ModelState.IsValid)
            {
                db.Entry(ticketManager).State = EntityState.Modified;
                db.SaveChanges();
                AlertHelper.setAlert("success", "Cập nhập thông tin vé bay thành công.");
                return RedirectToAction("Index");
            }
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "Pay" },
                new SelectListItem { Value = "1", Text = "Cancel" }
            };

            ViewBag.flight_schedules_id = new SelectList(db.FlightSchedules, "id", "code", ticketManager.flight_schedules_id);
            ViewBag.user_id = new SelectList(
               db.Users.Select(u => new
               {
                   id = u.id,
                   name_phone = u.name + " - " + u.phone_number
               }),
               "id",
               "name_phone",
               ticketManager.user_id
           );
            ViewBag.status = new SelectList(statusList, "Value", "Text", ticketManager.status);
            ViewBag.pay_id = new SelectList(db.Payments.Select(u => new { id = u.id, name = u.id + "-" + u.name_Payment + "-" + u.PayerID_Payment }), "id", "name", ticketManager.pay_id);
            //ViewBag.user_id = db.Users.Select(r => new SelectListItem() { Value = r.id.ToString(), Text = r.name + " " + r.phone_number });
            //ViewBag.DefaultUser = db.Users.Find(ticketManager.user_id);
            return View(ticketManager);
        }

        // GET: Admin/TicketManagers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketManager ticketManager = db.TicketManagers.Find(id);
            if (ticketManager == null)
            {
                return HttpNotFound();
            }
            return View(ticketManager);
        }

        // POST: Admin/TicketManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketManager ticketManager = db.TicketManagers.Find(id);
            db.TicketManagers.Remove(ticketManager);
            db.SaveChanges();
            DecreaseSeats(ticketManager.flight_schedules_id);
            ChangeSeats(ticketManager.flight_schedules_id, ticketManager.seat_location.ToString(), 1);
            AlertHelper.setAlert("success", "Xóa dữ liệu vé bay thành công.");
            return RedirectToAction("Index");
        }

        public void IncreaseSeats(int flight_schedules_id)
        {
            FlightSchedule flightSchedule = db.FlightSchedules.Find(flight_schedules_id);
            flightSchedule.bookedSeats += 1;
            if (ModelState.IsValid)
            {
                db.Entry(flightSchedule).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public void DecreaseSeats(int flight_schedules_id)
        {
            FlightSchedule flightSchedule = db.FlightSchedules.Find(flight_schedules_id);
            flightSchedule.bookedSeats -= 1;
            if (ModelState.IsValid)
            {
                db.Entry(flightSchedule).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public void ChangeSeats(int flight_schedules_id, String seated, int status)
        {
            Seats seat = db.Seats.FirstOrDefault(x => x.flight_schedules_id == flight_schedules_id && x.seat == seated.ToString());
            if (status == 1)
            {
                seat.isbooked = 0;
            }
            else
            {
                seat.isbooked = 1;
            }
            db.Entry(seat).State = EntityState.Modified;
            db.SaveChanges();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
