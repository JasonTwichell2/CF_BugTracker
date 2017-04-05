namespace BugSleuth2.Models
{
    using System;
    using System.Collections.Generic;

    public partial class TicketPriority : Lookup
    {
        public TicketPriority()
        {
            this.Tickets = new HashSet<Ticket>();
        }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
