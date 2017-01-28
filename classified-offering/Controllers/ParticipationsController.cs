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

        private ActionResult CheckAuthenticated(int? id)
        {
            User user = (User) Session["connectedUser"];

            bool hasAccess = false;
            if (user != null && id != null)
            {
                ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Find(id);
                hasAccess = user.ID == classifiedOffering.CreatorID || user.Role == 0;
            }
            
            return !hasAccess
                 ? Redirect("/User/SignIn")
                 : null;
        }

        // GET: Participations/Add
        public ActionResult Add(int? id)
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

            ActionResult notAuthenticated = CheckAuthenticated(id);
            if (notAuthenticated != null)
            {
                return notAuthenticated;
            }

            ViewBag.OffererID = new SelectList(db.Users, "ID", "Pseudo");
            return View();
        }

        // POST: Participations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OffererID")] Participation participation, int? id)
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
            ActionResult notAuthenticated = CheckAuthenticated(id);
            if (notAuthenticated != null)
            {
                return notAuthenticated;
            }

            if (ModelState.IsValid)
            {
                db.Participations.Add(participation);
                db.SaveChanges();
                return Redirect("/ClassifiedOfferings/Details/" + id);
            }

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
            ActionResult notAuthenticated = CheckAuthenticated(participation.ClassifiedOfferingID);
            if (notAuthenticated != null)
            {
                return notAuthenticated;
            }

            return View(participation);
        }

        // POST: Participations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Participation participation = db.Participations.Find(id);
            ActionResult notAuthenticated = CheckAuthenticated(participation.ClassifiedOfferingID);
            if (notAuthenticated != null)
            {
                return notAuthenticated;
            }
            db.Participations.Remove(participation);
            db.SaveChanges();
            return Redirect("/ClassifiedOfferings/Details/" + id);
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
