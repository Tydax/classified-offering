using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using classified_offering.Models;

namespace classified_offering.Controllers
{
    public class ParticipationsController : Controller
    {
        private ClassifiedOfferingDbContext db = new ClassifiedOfferingDbContext();

        // GET: Participations
        public ActionResult Index()
        {
            var participations = db.Participations.Include(p => p.ClassifiedOffering).Include(p => p.Offerer).Include(p => p.Receiver);
            return View(participations.ToList());
        }

        // GET: Participations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participation participation = db.Participations.Find(id);
            if (participation == null)
            {
                return HttpNotFound();
            }
            return View(participation);
        }

        // GET: Participations/Create
        public ActionResult Create()
        {
            ViewBag.ClassifiedOfferingID = new SelectList(db.ClassifiedOfferings, "ID", "Name");
            ViewBag.OffererID = new SelectList(db.Users, "ID", "Pseudo");
            ViewBag.ReceiverID = new SelectList(db.Users, "ID", "Pseudo");
            return View();
        }

        // POST: Participations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OffererID,ClassifiedOfferingID,ReceiverID")] Participation participation)
        {
            if (ModelState.IsValid)
            {
                db.Participations.Add(participation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClassifiedOfferingID = new SelectList(db.ClassifiedOfferings, "ID", "Name", participation.ClassifiedOfferingID);
            ViewBag.OffererID = new SelectList(db.Users, "ID", "Pseudo", participation.OffererID);
            ViewBag.ReceiverID = new SelectList(db.Users, "ID", "Pseudo", participation.ReceiverID);
            return View(participation);
        }

        // GET: Participations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participation participation = db.Participations.Find(id);
            if (participation == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClassifiedOfferingID = new SelectList(db.ClassifiedOfferings, "ID", "Name", participation.ClassifiedOfferingID);
            ViewBag.OffererID = new SelectList(db.Users, "ID", "Pseudo", participation.OffererID);
            ViewBag.ReceiverID = new SelectList(db.Users, "ID", "Pseudo", participation.ReceiverID);
            return View(participation);
        }

        // POST: Participations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OffererID,ClassifiedOfferingID,ReceiverID")] Participation participation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(participation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClassifiedOfferingID = new SelectList(db.ClassifiedOfferings, "ID", "Name", participation.ClassifiedOfferingID);
            ViewBag.OffererID = new SelectList(db.Users, "ID", "Pseudo", participation.OffererID);
            ViewBag.ReceiverID = new SelectList(db.Users, "ID", "Pseudo", participation.ReceiverID);
            return View(participation);
        }

        // GET: Participations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participation participation = db.Participations.Find(id);
            if (participation == null)
            {
                return HttpNotFound();
            }
            return View(participation);
        }

        // POST: Participations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Participation participation = db.Participations.Find(id);
            db.Participations.Remove(participation);
            db.SaveChanges();
            return RedirectToAction("Index");
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
