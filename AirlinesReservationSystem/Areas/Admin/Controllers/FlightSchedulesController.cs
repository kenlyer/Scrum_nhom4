using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirlinesReservationSystem.Models;
using AirlinesReservationSystem.Helper;

namespace AirlinesReservationSystem.Areas.Admin.Controllers
{
    public class FlightSchedulesController : Controller
    {
        private Model1 db = new Model1();

        //Hàm này trả về một View chứa danh sách tất cả các lịch trình chuyến bay hiện có trong cơ sở dữ liệu. Nó bao gồm thông tin về sân bay xuất phát, sân bay đích và máy bay dùng trong mỗi lịch trình chuyến bay.
        // GET: Admin/FlightSchedules
        public ActionResult Index()
        {
            var flightSchedules = db.FlightSchedules.Include(f => f.AirPort).Include(f => f.AirPort1).Include(f => f.Plane).OrderByDescending(f => f.id);
            return View(flightSchedules.ToList());
        }
        //Hàm này trả về thông tin chi tiết của một lịch trình chuyến bay dựa trên ID của lịch trình. Nếu không tìm thấy lịch trình chuyến bay với ID được cung cấp, nó trả về mã lỗi 404 (Not Found).
        // GET: Admin/FlightSchedules/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightSchedule flightSchedule = db.FlightSchedules.Find(id);
            if (flightSchedule == null)
            {
                return HttpNotFound();
            }
            return View(flightSchedule);
        }
        //Hàm này trả về một View cho phép người dùng tạo mới một lịch trình chuyến bay.
        // GET: Admin/FlightSchedules/Create
        public ActionResult Create()
        {
            viewbagitems();

            //ViewBag.from_airport = new SelectList(db.AirPorts, "id", "name");
            //ViewBag.to_airport = new SelectList(db.AirPorts, "id", "name");
            //ViewBag.plane_id = new SelectList(db.Planes, "id", "name");

            return View();
        }
        //Hàm này xử lý yêu cầu tạo mới một lịch trình chuyến bay. Nó kiểm tra tính hợp lệ của dữ liệu được gửi từ biểu mẫu và sau đó thêm lịch trình chuyến bay mới vào cơ sở dữ liệu nếu dữ liệu hợp lệ.
        // POST: Admin/FlightSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,code,plane_id,from_airport,to_airport,departures_at,arrivals_at,totalSeats,cost")] FlightSchedule flightSchedule)
        {
            //flightSchedule.totalSeats = 30;
            bool codes = db.FlightSchedules.Any(i => i.code ==  flightSchedule.code.ToString());
            TimeSpan timeDifference;
            if (flightSchedule.arrivals_at < flightSchedule.departures_at)
            {
                // Nếu giờ đến nhỏ hơn giờ đi, tức là qua ngày mới, cộng thêm 1 ngày vào thời gian đến
                timeDifference = (flightSchedule.arrivals_at + TimeSpan.FromDays(1)) - flightSchedule.departures_at;
            }
            else
            {
                // Trường hợp giờ đến không nhỏ hơn giờ đi
                timeDifference = flightSchedule.arrivals_at - flightSchedule.departures_at;
            }
            double TotalHours = timeDifference.TotalHours;
            // thêm trạng thái hoạt động
            flightSchedule.status_fs = "đang hoạt động";
            if (ModelState.IsValid)
            {
                if (flightSchedule.from_airport == flightSchedule.to_airport)// tránh trùng lắp điểm đi đến 
                {
                    ModelState.AddModelError("to_airport", "Sân bay đến không được trùng với sân bay đi.");
                    viewbagitems();
                    return View(flightSchedule);
                }
                else if (TotalHours >= 5)// tránh việc nhập giờ bay cao 
                {
                    ModelState.AddModelError("arrivals_at", "Chuyến bay không được quá 5 tiếng.");
                    viewbagitems();
                    return View(flightSchedule);
                }
                else if (codes)// mỗi code chỉ duy nhất 1
                {
                    ModelState.AddModelError("code", "chỉ 1 code duy nhất được tồn tại.");
                    viewbagitems();
                    return View(flightSchedule);
                }
                db.FlightSchedules.Add(flightSchedule);
                db.SaveChanges();
                FlightSchedule reload = db.FlightSchedules.FirstOrDefault(x => x.code == flightSchedule.code);
                CreateSeats(reload.id);
                AlertHelper.setAlert("success", "Tạo dữ liệu chuyến bay thành công.");
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var state in ModelState)
                {
                    var key = state.Key;  // Tên trường (field) bị lỗi
                    var errors = state.Value.Errors;  // Danh sách các lỗi
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                    }
                }
            }
            viewbagitems();
            return View(flightSchedule);
        }
        //Hàm này trả về một View cho phép người dùng chỉnh sửa thông tin của một lịch trình chuyến bay dựa trên ID của lịch trình.
        // GET: Admin/FlightSchedules/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlightSchedule flightSchedule = db.FlightSchedules.Find(id);
            if (flightSchedule == null)
            {
                return HttpNotFound();
            }
            //them status 
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "đang hoạt động", Text = "đang hoạt động" },
                new SelectListItem { Value = "không hoạt động", Text = "không hoạt động" }
            };

            TempData["oldcode"] = flightSchedule.code.ToString() ;
           

            ViewBag.from_airport = new SelectList(db.AirPorts, "id", "name", flightSchedule.from_airport);
            ViewBag.to_airport = new SelectList(db.AirPorts, "id", "name", flightSchedule.to_airport);
            ViewBag.plane_id = new SelectList(db.Planes, "id", "name", flightSchedule.plane_id);
            ViewBag.status_fs = new SelectList(statusList, "Text", "Value", flightSchedule.status_fs);
            return View(flightSchedule);
        }
        //Hàm này xử lý yêu cầu chỉnh sửa thông tin của một lịch trình chuyến bay. Nó kiểm tra tính hợp lệ của dữ liệu được gửi từ biểu mẫu và sau đó cập nhật thông tin của lịch trình chuyến bay trong cơ sở dữ liệu nếu dữ liệu hợp lệ.
        // POST: Admin/FlightSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code,plane_id,from_airport,to_airport,departures_at,arrivals_at,totalSeats,bookedSeats,availableSeats,status_fs,cost")] FlightSchedule flightSchedule)
        {
            var oldcode = TempData["oldcode"];
            int codes = db.FlightSchedules.Count(x=>x.code == flightSchedule.code.ToString().Trim());
            TimeSpan timeDifference;
            if (flightSchedule.arrivals_at < flightSchedule.departures_at)
            {
                // Nếu giờ đến nhỏ hơn giờ đi, tức là qua ngày mới, cộng thêm 1 ngày vào thời gian đến
                timeDifference = (flightSchedule.arrivals_at + TimeSpan.FromDays(1)) - flightSchedule.departures_at;
            }
            else
            {
                // Trường hợp giờ đến không nhỏ hơn giờ đi
                timeDifference = flightSchedule.arrivals_at - flightSchedule.departures_at;
            }

            double TotalHours = timeDifference.TotalHours;
            // kiểm tra nếu tổng số ghế mới < tổng số ghế đã được đặt thì báo lỗi ( ghế được đặt bao gồm ghế chưa thanh toán ) 
            if (ModelState.IsValid)
            {
                var statusList = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "đang hoạt động", Text = "đang hoạt động" },
                        new SelectListItem { Value = "không hoạt động", Text = "không hoạt động" }
                    };
                if (flightSchedule.totalSeats < flightSchedule.bookedSeats)
                {
                    ModelState.AddModelError("totalSeats", "Số lượng ghế phải lớn hơn số lượng ghế được đặt.");
                    viewbagitems();
                    ViewBag.status_fs = new SelectList(statusList, "Text", "Value", flightSchedule.status_fs);
                    return View(flightSchedule);
                }
                else if (flightSchedule.from_airport == flightSchedule.to_airport)
                {
                    ModelState.AddModelError("to_airport", "Sân bay đến không được trùng với sân bay đi.");
                    viewbagitems();
                    ViewBag.status_fs = new SelectList(statusList, "Text", "Value", flightSchedule.status_fs);
                    return View(flightSchedule);
                }
                else if(TotalHours >= 5)
                {
                    ModelState.AddModelError("arrivals_at", "Chuyến bay không được quá 5 tiếng.");
                    viewbagitems();
                    ViewBag.status_fs = new SelectList(statusList, "Text", "Value", flightSchedule.status_fs);
                    return View(flightSchedule);
                }
                else if (codes == 1 && oldcode.ToString() != flightSchedule.code )// mỗi code chỉ duy nhất 1
                {
                    ModelState.AddModelError("code", "chỉ 1 code duy nhất được tồn tại.");
                    viewbagitems();
                    ViewBag.status_fs = new SelectList(statusList, "Text", "Value", flightSchedule.status_fs);
                    return View(flightSchedule);
                }

            }

            if (ModelState.IsValid)
            {
                db.Entry(flightSchedule).State = EntityState.Modified;
                db.SaveChanges();
                AlertHelper.setAlert("success", "Cập nhập thông tin chuyến bay thành công.");
                return RedirectToAction("Index");
            }
            ViewBag.from_airport = new SelectList(db.AirPorts, "id", "name", flightSchedule.from_airport);
            ViewBag.to_airport = new SelectList(db.AirPorts, "id", "name", flightSchedule.to_airport);
            ViewBag.plane_id = new SelectList(db.Planes, "id", "name", flightSchedule.plane_id);
            return View(flightSchedule);
        }
        //Hàm này xử lý yêu cầu xóa một lịch trình chuyến bay khỏi cơ sở dữ liệu dựa trên ID của lịch trình được xác nhận. Nó xóa lịch trình chuyến bay khỏi cơ sở dữ liệu và chuyển hướng đến trang danh sách lịch trình chuyến bay.
        // GET: Admin/FlightSchedules/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try { 
            FlightSchedule flightSchedule = db.FlightSchedules.Find(id);
            db.FlightSchedules.Remove(flightSchedule);
            AlertHelper.setAlert("success", "Xóa dữ liệu chuyến bay thành công.");
            db.SaveChanges();
            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                 return RedirectToAction("Index");
            }
        }

        private void viewbagitems()
        {
            ViewBag.from_airport = new SelectList(db.AirPorts, "id", "name");
            ViewBag.to_airport = new SelectList(db.AirPorts, "id", "name");
            ViewBag.plane_id = new SelectList(db.Planes, "id", "name");
        }

        private void CreateSeats(int i)
        {
            for (int number = 1; number <= 30; number++)
            {
                Seats seats = new Seats();
                seats.seat = number.ToString();
                seats.flight_schedules_id = i;
                seats.isbooked = 0;
                db.Seats.Add(seats);
            }

            // Lưu tất cả các ghế vào database
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
