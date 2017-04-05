using PagedList;
using PagedList.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugSleuth2.Models
{
    public class TicketsViewModel
    {
        public IPagedList<Ticket> tickets { get; set; }
        public string projectName { get; set; }
        public string sortProperty { get; set; }
        public bool sortDirection { get; set; }
        public int? pageNumber { get; set; }
        public int pageSize { get; set; }
        public string userRole { get; set; }
        private string userId { get; set; }
        private int? projectId { get; set; }

        public TicketsViewModel(List<Ticket> tickets, string userId, string userRole, int? page)
        {
            this.pageSize = 10;
            this.pageNumber = page;
            this.userId = userId;
            this.userRole = userRole;
            this.tickets = tickets.ToPagedList((int)this.pageNumber, this.pageSize);
            refresh();
        }

        private void getProject()
        {
            if (projectId == null)
            {
//                this.projectId = 0;
            }
            else
            {
                this.projectId = projectId;
                this.projectName = new ApplicationDbContext().Projects.Find(projectId).Name;
            }
        }

        public object refresh()
        {
            this.tickets = tickets.ToPagedList((int)this.pageNumber, this.pageSize);

            getProject();

            if (userRole == "Admin") { }
            else if (userRole == "Project Manager")
            {
                this.tickets = this.tickets.Where(t => t.Project.Id == projectId).ToPagedList((int)this.pageNumber, this.pageSize);
            }
            else if (userRole == "Developer")
            {
                this.tickets = this.tickets.Where(t => t.AssignedUser.Id == userId).ToPagedList((int)this.pageNumber, this.pageSize);
            }
            else if (userRole == "Submitter")
            {
                this.tickets = this.tickets.Where(t => t.OwnerUser.Id == userId).ToPagedList((int)this.pageNumber, this.pageSize);
            }
            else
            {
                this.tickets = null;
                return null;
            }
            Sort("Created");
            return this;
        }

        public object Sort(string property)
        {
            // toggle sort direction
            if (sortProperty == property)
            {
                sortDirection = !sortDirection;
            }
            else
            {
                sortDirection = false;
            }
            sortProperty = property;

            // sort ticket list
            switch (property)
            {
                case "Title":
                    if (!sortDirection)
                        tickets = tickets.OrderBy(t => t.Title).ToPagedList((int)this.pageNumber, this.pageSize);
                    else
                        tickets = tickets.OrderByDescending(t => t.Title).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Created":
                    if (!sortDirection)
                        tickets = tickets.OrderBy(t => t.Created).ToPagedList((int)this.pageNumber, this.pageSize);
                    else
                        tickets = tickets.OrderByDescending(t => t.Created).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Last Updated":
                    if (!sortDirection)
                        tickets = tickets.OrderBy(t => t.Updated).ToPagedList((int)this.pageNumber, this.pageSize);
                    else
                        tickets = tickets.OrderByDescending(t => t.Updated).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Owner":
                    if (!sortDirection)
                        tickets = tickets.OrderBy(t => t.OwnerUser.DisplayName).ToPagedList((int)this.pageNumber, this.pageSize);
                    else
                        tickets = tickets.OrderByDescending(t => t.OwnerUser.DisplayName).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Assigned To":
                    if (!sortDirection)
                        tickets = tickets.OrderBy(t => t.AssignedUser.DisplayName).ToPagedList((int)this.pageNumber, this.pageSize);
                    else
                        tickets = tickets.OrderByDescending(t => t.AssignedUser.DisplayName).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Project":
                    if (!sortDirection)
                        tickets = tickets.OrderBy(t => t.Project.Name).ToPagedList((int)this.pageNumber, this.pageSize);
                    else
                        tickets = tickets.OrderByDescending(t => t.Project.Name).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Priority":
                    if (!sortDirection)
                        tickets = tickets.OrderBy(t => t.TicketPriority.Name).ToPagedList((int)this.pageNumber, this.pageSize);
                    else
                        tickets = tickets.OrderByDescending(t => t.TicketPriority.Name).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Status":
                    if (!sortDirection)
                        tickets = tickets.OrderBy(t => t.TicketStatus.Name).ToPagedList((int)this.pageNumber, this.pageSize);
                    else
                        tickets = tickets.OrderByDescending(t => t.TicketStatus.Name).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Type":
                    if (!sortDirection)
                        tickets = tickets.OrderBy(t => t.TicketType.Name).ToPagedList((int)this.pageNumber, this.pageSize);
                    else
                        tickets = tickets.OrderByDescending(t => t.TicketType.Name).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                default:
                    tickets = tickets.OrderBy(t => t.Created).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
            }
            return this;
        }


        public void Filter(string key, string value)
        {
            switch (key)
            {
                case "Owner":
                    tickets = tickets.Where(t => t.OwnerUserId == value).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Assigned":
                    tickets = tickets.Where(t => t.AssignedUserId == value).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Priority":
                    tickets = tickets.Where(t => t.TicketPriority.Name == value).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Status":
                    tickets = tickets.Where(t => t.TicketStatus.Name == value).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                case "Type":
                    tickets = tickets.Where(t => t.TicketType.Name == value).ToPagedList((int)this.pageNumber, this.pageSize);
                    break;
                default:
                    break;
            }
        }
    }
}