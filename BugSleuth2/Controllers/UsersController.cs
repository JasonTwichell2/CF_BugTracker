using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugSleuth2.Models;

namespace BugSleuth2.Controllers
{
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: ApplicationUsers
        public ActionResult Index()
        {
            var model = new List<UserViewModel>();
            var users = db.Users.ToList();
            foreach (var u in users)
            {
                var userView = BuildUserViewModel(u.Id);
                model.Add(userView);
            }
            return View(model);
        }

        // GET: ApplicationUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userView = BuildUserViewModel(id);
            if (userView == null)
            {
                return HttpNotFound();
            }
            return View(userView);
        }

        // GET: ApplicationUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userView = BuildUserViewModel(id); 
            if (userView == null)
            {
                return HttpNotFound();
            }
            return View(userView);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.Id);
                user.DisplayName = model.DisplayName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: ApplicationUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userView = BuildUserViewModel(id);
            if (userView == null)
            {
                return HttpNotFound();
            }
            return View(userView);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private UserViewModel BuildUserViewModel(string userId)
        {
            var rolesHelper = new UserRoleHelper();
            var projectsHelper = new UserProjectsHelper();

            var u = db.Users.Find(userId);
            var m = new UserViewModel();
            m.DisplayName = u.DisplayName;
            m.Email = u.Email;
            m.FirstName = u.FirstName;
            m.Id = u.Id;
            m.LastName = u.LastName;
            m.TicketsAssigned = db.Tickets.Where(t => t.AssignedUserId == u.Id).ToList();
            m.TicketsOwned = db.Tickets.Where(t => t.OwnerUserId == u.Id).ToList();
            m.UserProjects = projectsHelper.ListProjectsForUser(userId).ToList();            
            m.UserRoles = rolesHelper.ListUserRoles(u.Id).ToList();
            return m;
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
