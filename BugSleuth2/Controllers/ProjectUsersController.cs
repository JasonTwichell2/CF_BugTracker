using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugSleuth2.Models;

namespace BugSleuth2.Controllers
{
    public class ProjectUsersController : Controller
    {
        private UserProjectsHelper helper = new UserProjectsHelper();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AssignUsers
        public ActionResult AssignUsers(int id)
        {
            var model = new ProjectUsersViewModel();
            var project = db.Projects.Find(id);
            model.projectId = project.Id;
            model.projectName = project.Name;
            model.Users = new MultiSelectList(helper.ListUsersNotOnProject(id).OrderBy(u => u.DisplayName), "Id", "DisplayName");
            return View(model);
        }

        // POST: AssignUsers
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AssignUsers(ProjectUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    foreach (string id in model.SelectedUsers)
                    {
                        helper.AddUserToProject(id, model.projectId);
                    }
                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    // send error message back to view
                }
            }
            var userList = helper.ListUsersNotOnProject(model.projectId);
            model.Users = new MultiSelectList(userList.OrderBy(m => m.DisplayName), "Id", "DisplayName", null);
            return View(model);
        }

                // GET: UsersOnProject
        [Authorize(Roles = "Admin")]
        public ActionResult UsersOnProject(int id)
        {
            var model = new ProjectUsersViewModel();
            var project = db.Projects.Find(id);
            model.projectId = project.Id;
            model.projectName = project.Name;
            var userList = helper.ListUsersOnProject(id);
            model.Users = new MultiSelectList(userList.OrderBy(m => m.DisplayName), "Id", "DisplayName", null);
            return View(model);
        }


        // POST: UsersOnProject
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UsersOnProject(ProjectUsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    foreach (string id in model.SelectedUsers)
                    {
                        helper.RemoveUserFromProject(id, model.projectId);
                    }
                    return RedirectToAction("Index", "Projects");
                }
                else
                {
                    // send error message back to view
                }
            }
            var userList = helper.ListUsersOnProject(model.projectId);
            model.Users = new MultiSelectList(userList.OrderBy(m => m.DisplayName), "Id", "DisplayName", null);
            return View(model);
        }
    }
}