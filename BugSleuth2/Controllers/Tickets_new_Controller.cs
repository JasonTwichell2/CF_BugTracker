using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using BugSleuth.Models;

namespace BugSleuth.Controllers
{
    public class Tickets_New_Controller : Controller
    {
        private BugSleuthEntities db = new BugSleuthEntities();
        private ApplicationDbContext AppDb = new ApplicationDbContext();

        // Add a static project id variable to maintain project ticket list vs. complete list and initialize to zero
        private static int ProjectId = 0;

        // static variables to enable sorting by column heading
        private static bool SortDirection;
        private static string SortProperty;

        // Add a static model (tickets) to maintain sorting criteria between views and controller calls 
        private static IEnumerable<Ticket> tickets;

        // GET: Tickets
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Index(int? projectid, string param, string paramtype, int? page)
        {
            // if our projectid parameter is not null, assign its value to our static project id variable
            if (projectid != null)
            {
                ProjectId = (int)projectid;
            }

            tickets = db.Tickets.Include(t => t.BTUser).Include(t => t.BTUser1).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);

            /* if the project id is greater than zero...
             *  1) store it in ViewBag.pid
             *  2) create a heading (Tickets: Project X, for example), and put it in ViewBag.Title
             *  3) restrict the tickets list to just the tickets for the current project. 
             * otherwise...
             *  set the title (ViewBag.Title) to "Tickets" */
            if (ProjectId > 0)
            {
                ViewBag.pid = ProjectId;
                ViewBag.Title = "Tickets: " + db.Projects.Find(ProjectId).Name;
                tickets = tickets.Where(t => t.ProjectId == ProjectId);
            }
            else
            {
                ViewBag.Title = "Tickets";
            }
            
            /* Set up a switch statement to switch on the sort type criteria (paramtype)
             *  For each param type, restrict the tickets list to those tickets matching the
             *  parameter value (param) */
            switch(paramtype)
            {
                case "Owner":
                    tickets = tickets.Where(t => t.OwnerUserName == param);
                    break;
                case "Assigned":
                    tickets = tickets.Where(t => t.AssignedToUserName == param);
                    break;
                case "Priority":
                    tickets = tickets.Where(t => t.TicketPriority.Name == param);
                    break;
                case "Status":
                    tickets = tickets.Where(t => t.TicketStatus.Name == param);
                    break;
                case "Type":
                    tickets = tickets.Where(t => t.TicketType.Name == param);
                    break;
                default:
                    break;
            }

            switch (SortProperty)
            {
                case "Title":
                    if (!SortDirection)
                        tickets = tickets.OrderBy(t => t.Title);
                    else
                        tickets = tickets.OrderByDescending(t => t.Title);
                    break;
                case "Last Updated":
                    if (!SortDirection)
                        tickets = tickets.OrderBy(t => t.Updated);
                    else
                        tickets = tickets.OrderByDescending(t => t.Updated);
                    break;
                case "Owner":
                    if (!SortDirection)
                        tickets = tickets.OrderBy(t => t.OwnerUserName);
                    else
                        tickets = tickets.OrderByDescending(t => t.OwnerUserName);
                    break;
                case "Assigned To":
                    if (!SortDirection)
                        tickets = tickets.OrderBy(t => t.AssignedToUserName);
                    else
                        tickets = tickets.OrderByDescending(t => t.AssignedToUserName);
                    break;
                case "Project":
                    if (!SortDirection)
                        tickets = tickets.OrderBy(t => t.Project.Name);
                    else
                        tickets = tickets.OrderByDescending(t => t.Project.Name);
                    break;
                case "Priority":
                    if (!SortDirection)
                        tickets = tickets.OrderBy(t => t.TicketPriority.Name);
                    else
                        tickets = tickets.OrderByDescending(t => t.TicketPriority.Name);
                    break;
                case "Status":
                    if (!SortDirection)
                        tickets = tickets.OrderBy(t => t.TicketStatus.Name);
                    else
                        tickets = tickets.OrderByDescending(t => t.TicketStatus.Name);
                    break;
                case "Type":
                    if (!SortDirection)
                        tickets = tickets.OrderBy(t => t.TicketType.Name);
                    else
                        tickets = tickets.OrderByDescending(t => t.TicketType.Name);
                    break;
                default:
                    tickets = tickets.OrderBy(t => t.Created);
                    break;
            }

            int pagesize = 10;
            int pagenumber = (page ?? 1);
            if (User.IsInRole("Administrator"))
            {
                return View(tickets.ToPagedList(pagenumber, pagesize));
            }
            else if (User.IsInRole("Developer"))
            {
                return View(tickets.Where(t => t.AssignedToUserName == User.Identity.Name).ToPagedList(pagenumber, pagesize));
            }
            else
            {
                return View(tickets.Where(t => t.OwnerUserName == User.Identity.Name).ToPagedList(pagenumber, pagesize));
            }
        }

        // GET: Tickets/Details/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            // set ViewBag.pid equal to the current project id
            ViewBag.pid = ProjectId;
            ticket.Updated = DateTimeOffset.Now;
            ViewBag.OwnerUserName = new SelectList(db.BTUsers, "UserName", "DisplayName", ticket.OwnerUserName);
            ViewBag.AssignedToUserName = new SelectList(db.BTUsers, "UserName", "DisplayName", ticket.AssignedToUserName);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            Ticket model = new Ticket();
            DateTimeOffset date = DateTimeOffset.Now;
            model.Created = date;
            model.Updated = date;
            model.OwnerUserName = User.Identity.Name;
            
            // set ViewBag.pid equal to the current project id
            ViewBag.pid = ProjectId;
            ViewBag.OwnerUserName = new SelectList(db.BTUsers, "UserName", "DisplayName", model.OwnerUserName);
            ViewBag.AssignedToUserName = new SelectList(db.BTUsers.OrderBy(u => u.DisplayName), "UserName", "DisplayName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Submitter")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserName,AssignedToUserName")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                // add the ticket to the DB and save the changes
                db.Tickets.Add(ticket);
                db.SaveChanges();

                // notify the assigned user of the new ticket
                SendNotification(ticket);

                return RedirectToAction("Index", "Tickets");
            }

            ViewBag.OwnerUserName = new SelectList(db.BTUsers, "UserName", "DisplayName", ticket.OwnerUserName);
            ViewBag.AssignedToUserName = new SelectList(db.BTUsers, "UserName", "DisplayName", ticket.AssignedToUserName);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        public ActionResult Edit(int? id, int? projectid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            
            // set ViewBag.pid equal to the current project id
            ViewBag.pid = ProjectId;
            ticket.Updated = DateTimeOffset.Now;
            ViewBag.OwnerUserName = new SelectList(db.BTUsers, "UserName", "DisplayName", ticket.OwnerUserName);
            ViewBag.AssignedToUserName = new SelectList(db.BTUsers, "UserName", "DisplayName", ticket.AssignedToUserName);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrator, Developer, Submitter")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserName,AssignedToUserName")] Ticket ticket, int? projectid)
        {
            if (ModelState.IsValid)
            {
                // retrieve the old version of the ticket from the DB
                var oldTicket = db.Tickets.Find(ticket.Id);

                // determine what's changed and for each changed property, add a new TicketHistory entry 
                //  to the DB and save the changes again
                if (oldTicket.Description != ticket.Description)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Description",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.Description,
                        NewValue = ticket.Description
                    });
                }
                if (oldTicket.ProjectId != ticket.ProjectId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Project",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.Project.Name,
                        NewValue = db.Projects.Find(ticket.ProjectId).Name
                    });
                }
                if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Type",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = db.Projects.Find(ticket.TicketTypeId).Name
                    });
                }
                if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Priority",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.TicketPriority.Name,
                        NewValue = db.Projects.Find(ticket.TicketPriorityId).Name
                    });
                }
                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Status",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.TicketStatus.Name,
                        NewValue = db.Projects.Find(ticket.TicketStatusId).Name
                    });
                }
                if (oldTicket.AssignedToUserName != ticket.AssignedToUserName)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Assigned",
                        Changed = DateTimeOffset.Now,
                        UserName = User.Identity.Name,
                        TicketId = ticket.Id,
                        OldValue = oldTicket.AssignedToUserName,
                        NewValue = ticket.AssignedToUserName
                    });
                }
                // save the changes
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                // Notify the assigned user of the changes
                SendNotification(ticket);

                return RedirectToAction("Index");
            }
            ViewBag.OwnerUserName = new SelectList(db.BTUsers, "UserName", "DisplayName", ticket.OwnerUserName);
            ViewBag.AssignedToUserName = new SelectList(db.BTUsers, "UserName", "DisplayName", ticket.AssignedToUserName);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        
        public ActionResult Sort(string property, IEnumerable<Ticket> model)
        {
            // set the tickets list equal to the incoming model
            tickets = model;

            if (SortProperty == property)
            {
                // toggle direction
                SortDirection = !SortDirection;
            }
            else
            {
                // initial direction (ascending)
                SortDirection = false;
            }

            SortProperty = property;
            return RedirectToAction("Index");
        }


        public void SendNotification(Ticket ticket)
        {
            if (ticket.AssignedToUserName != null)
            {
                MailMessage mail = new MailMessage();
                var toAddress = AppDb.Users.FirstOrDefault(u => u.UserName == ticket.AssignedToUserName).Email;
                mail.To.Add(new MailAddress(toAddress));
                mail.From = new MailAddress("ajensen.home@gmail.com");
                if (ticket.Id > 0)
                {
                    mail.Subject = "Updated Ticket Assignment: Priority " + db.TicketPriorities.Find(ticket.TicketPriorityId).Name;
                    mail.Body = "One of your assigned tickets, Ticket Id #" + ticket.Id + ", has been updated. Please log in and review the updated information as soon as possible.\n\nThank you,\nBugSleuth Administration";
                }
                else
                {
                    mail.Subject = "New Ticket Assignment: Priority " + db.TicketPriorities.Find(ticket.TicketPriorityId).Name;
                    mail.Body = "A new ticket has been assigned to you. Please log in and view your existing tickets to retrieve this new assignment.\n\nThank you,\nBugSleuth Administration";
                }
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Credentials = new System.Net.NetworkCredential("ajensen.home@gmail.com", "spurlock08");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                db.TicketNotifications.Add(new TicketNotification { TicketId = ticket.Id, UserName = ticket.AssignedToUserName });
                db.SaveChanges();
            }
        }
    }
}
