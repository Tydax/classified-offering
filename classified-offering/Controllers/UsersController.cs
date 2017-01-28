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
    public class UsersController : Controller
    {
        private ClassifiedOfferingDbContext db = new ClassifiedOfferingDbContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/SignUp
        public ActionResult SignUp()
        {
            return View();
        }

        // POST: Users/SignUp
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Pseudo,FirstName,LastName,Email,Password")] User user)
        {
            if (db.Users.First(u => u.Pseudo == user.Pseudo) != null)
            {
                return View(user);
            }

            if (ModelState.IsValid)
            {
                user.Role = user.Pseudo == "mollis"
                          ? 0
                          : 1;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/SignIn
        public ActionResult SignIn()
        {
            return View();
        }

        // POST: Users/SignIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Auth([Bind(Include = "Pseudo,Password")] User user)
        {
            User userDb = db.Users.First(u => u.Pseudo == user.Pseudo && u.Password == user.Password);

            if (userDb != null)
            {
                Session["connectedUser"] = userDb;
                return Redirect("/ClassifiedOfferings");
            }

            return View(user)
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
