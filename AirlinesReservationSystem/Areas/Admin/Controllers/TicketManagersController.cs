using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AirlinesReservationSystem.Models;

namespace AirlinesReservationSystem.Areas.Admin.Controllers
{
    public class TicketManagersController : Controller
    {
        private Model1 db = new Model1();

        // GET: Admin/TicketManagers
        public ActionResult Index()
        {
            var ticketManagers = db.TicketManagers.Include(t => t.FlightSchedule).Include(t => t.User);
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
            ViewBag.user_id = new SelectList(db.Users, "id", "name");
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
            if (ModelState.IsValid)
            {
                ticketManager.code = a.code;
                db.TicketManagers.Add(ticketManager);
                db.SaveChanges();

                IncreaseSeats(ticketManager.flight_schedules_id);
                return RedirectToAction("Index");
            }

            ViewBag.flight_schedules_id = new SelectList(db.FlightSchedules, "id", "id", ticketManager.flight_schedules_id);
            ViewBag.user_id = new SelectList(db.Users, "id", "name", ticketManager.user_id);
            return View(ticketManager);
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
            ViewBag.flight_schedules_id = new SelectList(db.FlightSchedules, "id", "code", ticketManager.flight_schedules_id);
            ViewBag.user_id = new SelectList(db.Users, "id", "name", ticketManager.user_id);
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
                return RedirectToAction("Index");
            }
            ViewBag.flight_schedules_id = new SelectList(db.FlightSchedules, "id", "id", ticketManager.flight_schedules_id);
            ViewBag.user_id = new SelectList(db.Users, "id", "name", ticketManager.user_id);
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
