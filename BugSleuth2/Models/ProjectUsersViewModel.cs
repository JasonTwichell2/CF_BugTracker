using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BugSleuth2.Models
{
    public class ProjectUsersViewModel
    {
        public int projectId { get; set; }
        public string projectName { get; set; }
        [Display(Name = "Available Users")]
        public System.Web.Mvc.MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }
    }
}