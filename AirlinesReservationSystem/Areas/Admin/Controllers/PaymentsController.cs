using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirlinesReservationSystem.Models;


namespace AirlinesReservationSystem.Areas.Admin.Controllers
{
    public class PaymentsController : Controller
    {
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
            if (ModelState.IsValid)
            {
                db.Payments.Add(payments);
                db.SaveChanges();
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
            if (ModelState.IsValid)
            {
                db.Entry(payments).State = EntityState.Modified;
                db.SaveChanges();
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
            return RedirectToAction("Index");
        }
    }
}