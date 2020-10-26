using System;
using System.Collections.Generic;

namespace Orleans.Interfaces
{
    public class Attendant
    {
        public string AttendantName { get; set; }
        public Dictionary<string, IList<string>> BotsInformation { get; set; }
        public IList<Ticket> CurrentTickets { get; set; }
        public int MaxSlots { get; set; }
        public DateTime LastTicketReceivedDate { get; set; }
    }
}
