using Microsoft.AspNet.Identity.EntityFramework; // needed for IdentityRole
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BugSleuth2.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "Display name")]
        public string DisplayName { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Email / Login")]
        public string Email { get; set; }

        [Display(Name = "User Roles")]
        public List<string> UserRoles { get; set; }

        [Display(Name = "Assigned Projects")]
        public List<Project> UserProjects { get; set; }

        [Display(Name = "Submitted Tickets")]
        public List<Ticket> TicketsOwned { get; set; }

        [Display(Name = "Assigned Tickets")]
        public List<Ticket> TicketsAssigned { get; set; }
    }
}
