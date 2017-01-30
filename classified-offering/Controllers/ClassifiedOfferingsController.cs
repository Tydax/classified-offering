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

        private ActionResult CheckAuthenticated(int? id = null)
        {
            User user = (User)Session["connectedUser"];

            if (user == null)
            {
                return Redirect("/User/SignIn");
            }

            if (id != null && user.ID != id && user.Role != 0)
            {
                return RedirectToAction("Forbidden", "Home");
            }

            return null;
        }

        // GET: ClassifiedOfferings
        public ActionResult Index()
        {
            ActionResult NotAuthenticated = CheckAuthenticated();
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }
            User connectedUser = (User) Session["connectedUser"];
            var classifiedOfferings = connectedUser.Role == 0
                ? db.ClassifiedOfferings.ToList()
                : db.ClassifiedOfferings.Where(c => c.CreatorID == connectedUser.ID).ToList();
            
            return View(classifiedOfferings);
        }

        // GET: ClassifiedOfferings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Include(co => co.Participations).First(co => co.ID == id);
            if (classifiedOffering == null)
            {
                return HttpNotFound();
            }
            return View(classifiedOffering);
        }

        // GET: ClassifiedOfferings/Create
        public ActionResult Create()
        {
            ActionResult NotAuthenticated = CheckAuthenticated();
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }
            return View();
        }

        // POST: ClassifiedOfferings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,CreationDate,CreatorID")] ClassifiedOffering classifiedOffering)
        {
            ActionResult NotAuthenticated = CheckAuthenticated();
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }

            if (ModelState.IsValid)
            {
                // Update classified offering
                classifiedOffering.CreatorID = ((User) Session["connectedUser"]).ID;
                classifiedOffering.CreationDate = DateTime.Now;
                classifiedOffering.isLocked = false;
                db.ClassifiedOfferings.Add(classifiedOffering);
                // Create participation for creator
                Participation participation = new Participation();
                participation.OffererID = classifiedOffering.CreatorID;
                db.Participations.Add(participation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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

            ActionResult NotAuthenticated = CheckAuthenticated(classifiedOffering.CreatorID);
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }
            
            return View(classifiedOffering);
        }

        // POST: ClassifiedOfferings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,CreationDate,CreatorID")] ClassifiedOffering classifiedOffering)
        {
            ActionResult NotAuthenticated = CheckAuthenticated(classifiedOffering.CreatorID);
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }
            if (ModelState.IsValid)
            {
                db.Entry(classifiedOffering).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
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
            ActionResult NotAuthenticated = CheckAuthenticated(classifiedOffering.CreatorID);
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }
            return View(classifiedOffering);
        }

        // POST: ClassifiedOfferings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Find(id);
            ActionResult NotAuthenticated = CheckAuthenticated(classifiedOffering.CreatorID);
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }
            db.ClassifiedOfferings.Remove(classifiedOffering);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: ClassifiedOfferings/Lock
        public ActionResult Lock(int? id)
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
            ActionResult NotAuthenticated = CheckAuthenticated(classifiedOffering.CreatorID);
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }
            return View(classifiedOffering);
        }

        // POST: ClassifiedOfferings/Lock
        [HttpPost, ActionName("Lock")]
        [ValidateAntiForgeryToken]
        public ActionResult Lock(int id)
        {
            ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Include(co => co.Participations).First(co => co.ID == id);
            ActionResult NotAuthenticated = CheckAuthenticated(classifiedOffering.CreatorID);
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }
            if (!classifiedOffering.isLocked)
            {
                classifiedOffering.isLocked = true;
                generateParticipations(classifiedOffering);
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = id });
        }


        private static Random RNG = new Random();

        private void generateParticipations(ClassifiedOffering co)
        {
            ICollection<Participation> participations = co.Participations;
            IList<User> participants = participations.Select(p => p.Offerer).ToList<User>();

            // Shuffle list
            int n = participants.Count;
            while (n > 1)
            {
                n--;
                int k = RNG.Next(n + 1);
                User value = participants[k];
                participants[k] = participants[n];
                participants[n] = value;
            }

            for (n = 0; n < participants.Count; n++)
            {
                User off = participants[n];
                User rec = n == participants.Count - 1
                         ? participants[0]
                         : participants[n + 1];
                Participation part = participations.Where(p => p.OffererID == off.ID).First();
                part.ReceiverID = rec.ID;
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
