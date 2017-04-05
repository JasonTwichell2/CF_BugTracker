using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using BugSleuth2.Models;

namespace BugSleuth2.Controllers
{
    public class RolesController : Controller
    {
        // create a private member variable for this controller class (you will need these throughout the controllers here) that connects to the AspNet tables (this is represented by the ApplicationDbContext object)
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRoleHelper helper = new UserRoleHelper();
        // GET: Roles
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            List<AdminUserViewModel> userList = new List<AdminUserViewModel>();
            foreach (var users in db.Users.ToList())
            {
                var userCollection = new AdminUserViewModel();
                userCollection.User = users;
                userCollection.role = helper.ListUserRoles(users.Id).ToList();
                userList.Add(userCollection);
            }
            return View(userList);
        }
        //GET: Admin/SelectRole/5
        public ActionResult SelectRole(string id)
        {
            var user = db.Users.Find(id);
            var roleUser = new UserRolesViewModel();
            roleUser.Id = user.Id;
            roleUser.FirstName = user.FirstName;
            roleUser.LastName = user.LastName;
            roleUser.DisplayName = user.DisplayName;
            roleUser.SelectedRoles = helper.ListUserRoles(user.Id).ToArray();
            roleUser.UserRoles = new MultiSelectList(db.Roles, "Name", "Name", roleUser.SelectedRoles);

            return View(roleUser);
        }

        //POST: Admin/SelectRoles/5
        [HttpPost]
        public ActionResult SelectRole(UserRolesViewModel model)
        {
            var user = db.Users.Find(model.Id);
            foreach (var rolermv in db.Roles.Select(r => r.Name).ToList())
            //foreach (var rolermv in user.Roles.ToList())
            {
                helper.RemoveUserFromRole(user.Id, rolermv);
            }

            foreach (var roleadd in model.SelectedRoles)
            {
                helper.AddUserToRole(user.Id, roleadd);
            }

            ViewBag.Confirm = "User's role has been successfully modified";
            return RedirectToAction("Index");
        }

    }
}
