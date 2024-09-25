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
        //Hàm này xử lý thanh toán vé máy bay. Nó kiểm tra xem người dùng đã đăng nhập chưa. Nếu đã đăng nhập, nó tạo một hoặc nhiều vé máy bay dựa trên thông tin được cung cấp và lưu chúng vào cơ sở dữ liệu. Sau đó, nó trả về một thông báo JSON với trạng thái thanh toán.
        [HttpGet]
        public ActionResult PayTicket(string ticketID,int flightScheduleID,int amount = 1)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            response["status"] = "200";
            response["message"] = "";
            if (!AuthHelper.isLogin())
            {
                response["status"] = "400";
                response["message"] = "Phải đăng nhập mới có thể mua được vé.";
                return Content(JsonConvert.SerializeObject(response));
            }
            for (int i = 0; i < amount; i++)
            {
                TicketManager ticket = new TicketManager();
                ticket.user_id = AuthHelper.getIdentity().id;
                ticket.flight_schedules_id = flightScheduleID;
                ticket.status = TicketManager.STATUS_PAY;
                ticket.code = ticketID+""+i.ToString();
                if (ModelState.IsValid)
                {
                    db.TicketManagers.Add(ticket);
                    db.SaveChanges();
                }
                else
                {
                    AlertHelper.setToast("danger", "Đặt vé không thành công.");
                }
            }
            AlertHelper.setToast("success", "Đặt vé thành công.");
            return Content(JsonConvert.SerializeObject(response));
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
        //Hàm này hủy một vé máy bay dựa trên ID của vé.
        public ActionResult CancelTicket(int id)
        {
            TicketManager ticket = db.TicketManagers.Where(s => s.id == id).FirstOrDefault();
            if(ticket == null)
            {
                return HttpNotFound();
            }
            ticket.status = TicketManager.STATUS_CANCEL;
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                AlertHelper.setToast("warning", "Hủy vé thành công.");
            }
            return RedirectToAction("YourTicket", "Home");
        }
       // Hàm này trả về một View để chỉnh sửa thông tin người dùng dựa trên ID được cung cấp.
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //Hàm này xử lý yêu cầu chỉnh sửa thông tin người dùng. Nó kiểm tra ModelState, sau đó cập nhật thông tin người dùng trong cơ sở dữ liệu và chuyển hướng đến trang chỉnh sửa người dùng.
        // POST: Admin/Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Include = "id,name,email,cccd,address,phone_number,password,user_type")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                AlertHelper.setToast("success", "Cập nhập thông tin khách hàng thành công.");
                return RedirectToAction("EditUser");
            }
            return View(user);
        }

        // Hàm này xử lý yêu cầu thay đổi mật khẩu của người dùng. Nó kiểm tra mật khẩu cũ, sau đó cập nhật mật khẩu mới trong cơ sở dữ liệu và trả về một thông báo JSON với kết quả.
        public ActionResult ChangePassword(string old_password,string new_password)
        {
            Dictionary<string, string> response = new Dictionary<string, string>();
            response["status"] = "200";
            response["message"] = "";
            User identity = AuthHelper.getIdentity();
            if(identity.password != old_password)
            {
                response["status"] = "400";
                response["message"] = "Sai thông tin mật khẩu cũ.";
                return Content(JsonConvert.SerializeObject(response));
            }
            User user = db.Users.Find(identity.id);
            if (user != null) {
                user.password = new_password;
                db.SaveChanges();
                response["status"] = "200";
                response["message"] = "Đổi mật khẩu thành công.";
                AlertHelper.setToast("success", "Đổi mật khẩu thành công");
            }
            return Content(JsonConvert.SerializeObject(response));
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