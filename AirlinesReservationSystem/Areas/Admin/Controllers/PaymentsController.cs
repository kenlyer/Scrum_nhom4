using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using AirlinesReservationSystem.Helper;
using AirlinesReservationSystem.Models;


namespace AirlinesReservationSystem.Areas.Admin.Controllers
{
    public class PaymentsController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (AuthHelper.getIdentityEmployeee() == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { Controller = "Security", Action = "Login" }));
            }
            base.OnActionExecuted(filterContext);
        }
        private Model1 db = new Model1();
        // GET: Admin/Payments
        public ActionResult Index()
        {
            var payments = db.Payments.OrderByDescending(x => x.id).ToList();
            return View(payments);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            return View(payments);
        }

        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "id", "name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,email_Payment,name_Payment,PayerID_Payment,UserID")] Payments payments)
        {
            if (payments == null)
            {
                ViewBag.UserID = new SelectList(db.Users, "id", "name");
                ModelState.AddModelError("email_Payment", "Sai định dạng email hoặc chưa nhập vui lòng kiểm tra.");
                return View();
            }
            if (!CheckEmail(payments.email_Payment))
            {
                ViewBag.UserID = new SelectList(db.Users, "id", "name");
                ModelState.AddModelError("email_Payment", "Sai định dạng email hoặc chưa nhập vui lòng kiểm tra.");
                return View(payments);
            }
            if (ModelState.IsValid)
            {
                db.Payments.Add(payments);
                db.SaveChanges();
                AlertHelper.setAlert("success", "Tạo phiếu thanh toán thành công.");
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "id", "name");
            return View();
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserID = new SelectList(db.Users, "id", "name");
            return View(payments);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,email_Payment,name_Payment,PayerID_Payment,UserID")] Payments payments)
        {
            if (payments == null)
            {
                ViewBag.UserID = new SelectList(db.Users, "id", "name");
                ModelState.AddModelError("email_Payment", "Sai định dạng email hoặc chưa nhập vui lòng kiểm tra.");
                return View();
            }
            if (!CheckEmail(payments.email_Payment))
            {
                ViewBag.UserID = new SelectList(db.Users, "id", "name");
                ModelState.AddModelError("email_Payment", "Sai định dạng email hoặc chưa nhập vui lòng kiểm tra.");
                return View(payments);
            }

            if (ModelState.IsValid)
            {
                db.Entry(payments).State = EntityState.Modified;
                db.SaveChanges();
                AlertHelper.setAlert("success", "Cập nhập phiếu thanh toán thành công.");
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.Users, "id", "name");
            return View(payments);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payments payments = db.Payments.Find(id);
            if (payments == null)
            {
                return HttpNotFound();
            }
            return View(payments);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payments payments = db.Payments.Find(id);
            db.Payments.Remove(payments);
            db.SaveChanges();
            AlertHelper.setAlert("success", "Xóa phiếu thanh toán thành công.");
            return RedirectToAction("Index");
        }

        //Components
        public bool CheckEmail(string email)
        {
            if (email == null)
            {
                return false;
            }
            // Regex để kiểm tra địa chỉ email với nhiều miền khác nhau
            string regex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Kiểm tra xem email có khớp với regex không
            return Regex.IsMatch(email, regex);
        }
    }
}