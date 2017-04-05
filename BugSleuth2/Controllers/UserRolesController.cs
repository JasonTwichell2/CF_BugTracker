using System.Linq;
using System.Web.Mvc;
using BugSleuth2.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugSleuth2.Controllers
{
    public class UserRolesController : Controller
    {
        //private ApplicationDbContext db = new ApplicationDbContext();
        //private UserRoleHelper helper = new UserRoleHelper();

        //// GET: AssignUsers
        //[Authorize(Roles = "Admin")]
        //public ActionResult AssignUsers(string id)
        //{
        //    var role = db.Roles.Find(id);
        //    var model = new UserRolesViewModel();
        //    model.Id = role.Id;
        //    model.RoleName = role.Name;
        //    var userList = helper.UsersNotInRole(role.Name);
        //    model.User = new MultiSelectList(userList.OrderBy(m => m.DisplayName), "Id", "DisplayName", null);
        //    return View(model);
        //}

        //// POST: AssignUsers
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //[ValidateAntiForgeryToken]
        //public ActionResult AssignUsers(UserRolesViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (model.SelectedUsers != null)
        //        {
        //            foreach (string id in model.SelectedUsers)
        //            {
        //                var user = db.Users.Find(id);
        //                helper.AddUserToRole(id, model.RoleName);
        //            }
        //            return RedirectToAction("Index", "Roles");
        //        }
        //    }
        //    return View(model);
        //}

        //// GET: UsersInRole
        //[Authorize(Roles = "Admin")]
        //public ActionResult UsersInRole(string id)
        //{
        //    var role = db.Roles.Find(id);
        //    var model = new UserRolesViewModel();
        //    model.Id = role.Id;
        //    model.RoleName = role.Name;
        //    var userList = helper.UsersInRole(role.Name);
        //    model.User = new MultiSelectList(userList.OrderBy(m => m.DisplayName), "Id", "DisplayName", null);
        //    return View(model);
        //}


        //// POST: UsersInRole
        //[HttpPost]
        //[Authorize(Roles="Admin")]
        //[ValidateAntiForgeryToken]
        //public ActionResult UsersInRole(UserRolesViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (model.SelectedUsers != null)
        //        {
        //            foreach (string id in model.SelectedUsers)
        //            {
        //                //helper.RemoveUserFromRole(id, model.RoleName);
        //                var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
        //                manager.RemoveFromRoleAsync(id, model.RoleName);
        //            }
        //            return RedirectToAction("Index", "Roles");
        //        }
        //    }
        //    return View(model);
        //}
    }
}