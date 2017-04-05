using Microsoft.AspNet.Identity.EntityFramework; // needed for IdentityRole
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BugSleuth2.Models
{
    public class AdminUserViewModel
    {
        public ApplicationUser User { get; set; }
        public List<string> role { get; set; }
    }
    public class UserRolesViewModel
    {
        public string Id { get; set; }

        public List<string> RoleName { get; set; }

        public ApplicationUser User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        
        public MultiSelectList UserRoles { get; set; }
        public string[] SelectedRoles { get; set; }
    }
}