using Orleans.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    public class QueueGrain : Grain, IQueueGrain
    {
        private Queue<Ticket> Tickets { get; set; }
        private Dictionary<string, AttendantRecord> Attendants { get; set; }

        public override Task OnActivateAsync()
        {
            Tickets = new Queue<Ticket>();
            Attendants = new Dictionary<string, AttendantRecord>();

            return base.OnActivateAsync();
        }

        public async Task AddToQueueAsync(Ticket ticket)
        {
            await this.EnqueueTicketAsync(ticket);
        }

        public async Task RegisterAttendantAsync(string attendantIdentity, int maxSlots, int inAttendance)
        {
            if (!Attendants.ContainsKey(attendantIdentity))
            {
                Attendants.Add(attendantIdentity, new AttendantRecord(attendantIdentity, maxSlots, inAttendance));
            }
        }

        public async Task UnRegisterAttendantAsync(string attendantIdentity)
        {
            Attendants.Remove(attendantIdentity);
        }

        public async Task DistributeTicketAsync()
        {
            var ticket = this.Tickets.Dequeue();
            var availableAttendant = GetAvailableAttendant();

            if (ticket != null && availableAttendant != null)
            {
                await this.SendTicketToAttendantAsync(ticket, availableAttendant.Identity);
            }
        }

        private async Task EnqueueTicketAsync(Ticket ticket)
        {
            this.Tickets.Enqueue(ticket);
        }

        private async Task SendTicketToAttendantAsync(Ticket ticket, string attendantName)
        {
            var currentDate = DateTime.Now;

            var attendant = GrainFactory.GetGrain<IAttendantGrain>($"{attendantName}");
            await attendant.AssignTicketAsync(ticket, currentDate);

            Attendants[attendantName].InAttendance++;
            Attendants[attendantName].LastTicketReceivedDate = currentDate;
        }

        private AttendantRecord GetAvailableAttendant()
        {
            return Attendants
                    .Values
                    .Where(attendant => attendant.IsAvailable)
                    .OrderBy(attendant => attendant.LastTicketReceivedDate)
                    .FirstOrDefault();
        }
    }
}
