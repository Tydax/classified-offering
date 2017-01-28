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
            return user == null || (id != null && user.ID != id && user.Role != 0)
                 ? Redirect("/User/SignIn")
                 : null;
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
                classifiedOffering.CreatorID = ((User) Session["connectedUser"]).ID;
                db.ClassifiedOfferings.Add(classifiedOffering);
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
            ActionResult NotAuthenticated = CheckAuthenticated();
            if (NotAuthenticated != null)
            {
                return NotAuthenticated;
            }
            ClassifiedOffering classifiedOffering = db.ClassifiedOfferings.Find(id);
            User connectedUser = (User) Session["connectedUser"];
            if (connectedUser.ID != classifiedOffering.CreatorID || connectedUser.Role != 0)
            {
                return NotAuthenticated;
            }
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
