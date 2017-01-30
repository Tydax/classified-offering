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

        private ActionResult CheckAuthenticated(int? id = null)
        {
            User user = (User) Session["connectedUser"];

            if (user == null)
            {
                return RedirectToAction("SignIn", "User");
            }

            if (id != null)
            {
                ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Find(id);
                if (user.ID != classifiedOffering.CreatorID && user.Role != 0)
                {
                    return RedirectToAction("Forbidden", "Home");
                }
            }
            
            return null;
        }

        // GET: Participations/Index
        public ActionResult Index()
        {
            ActionResult notAuthenticated = CheckAuthenticated();
            if (notAuthenticated != null)
            {
                return notAuthenticated;
            }
            User user = (User)Session["connectedUser"];
            ICollection<Participation> parts = db.Participations.Include(part => part.Receiver).Where(part => part.OffererID == user.ID).ToList();
            return View(parts);
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
            Participation part = new Participation();
            part.ClassifiedOfferingID = (int) id;
            ViewBag.OffererID = new SelectList(db.Users, "ID", "Pseudo");
            return View(part);
        }

        // POST: Participations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "ID,OffererID")] Participation participation, int? id)
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
                participation.ClassifiedOfferingID = (int) id;
                db.Participations.Add(participation);
                db.SaveChanges();
                return RedirectToAction("Details", "ClassifiedOfferings", new { id = id });
            }

            ViewBag.ReceiverID = new SelectList(db.Users, "ID", "Pseudo", participation.ReceiverID);
            return View(participation);
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
