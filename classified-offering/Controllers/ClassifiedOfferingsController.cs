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
    public class ClassifiedOfferingsController : Controller
    {
        private ClassifiedOfferingDbContext db = new ClassifiedOfferingDbContext();

        // GET: ClassifiedOfferings
        public ActionResult Index()
        {
            var classifiedOfferings = db.ClassifiedOfferings.Include(c => c.Creator);
            return View(classifiedOfferings.ToList());
        }

        // GET: ClassifiedOfferings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Find(id);
            if (classifiedOffering == null)
            {
                return HttpNotFound();
            }
            return View(classifiedOffering);
        }

        // GET: ClassifiedOfferings/Create
        public ActionResult Create()
        {
            ViewBag.CreatorID = new SelectList(db.Users, "ID", "Pseudo");
            return View();
        }

        // POST: ClassifiedOfferings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,CreationDate,CreatorID")] ClassifiedOffering classifiedOffering)
        {
            if (ModelState.IsValid)
            {
                db.ClassifiedOfferings.Add(classifiedOffering);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CreatorID = new SelectList(db.Users, "ID", "Pseudo", classifiedOffering.CreatorID);
            return View(classifiedOffering);
        }

        // GET: ClassifiedOfferings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Find(id);
            if (classifiedOffering == null)
            {
                return HttpNotFound();
            }
            ViewBag.CreatorID = new SelectList(db.Users, "ID", "Pseudo", classifiedOffering.CreatorID);
            return View(classifiedOffering);
        }

        // POST: ClassifiedOfferings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,CreationDate,CreatorID")] ClassifiedOffering classifiedOffering)
        {
            if (ModelState.IsValid)
            {
                db.Entry(classifiedOffering).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CreatorID = new SelectList(db.Users, "ID", "Pseudo", classifiedOffering.CreatorID);
            return View(classifiedOffering);
        }

        // GET: ClassifiedOfferings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Find(id);
            if (classifiedOffering == null)
            {
                return HttpNotFound();
            }
            return View(classifiedOffering);
        }

        // POST: ClassifiedOfferings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Find(id);
            db.ClassifiedOfferings.Remove(classifiedOffering);
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
