using Orleans.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    public class AttendantGrain : Grain, IAttendantGrain
    {
        private Attendant AttendantData;

        public async Task GoOnlineAsync(Attendant attendant)
        {
            AttendantData = attendant;

            var tasks = AttendantData.BotsInformation.SelectMany(
                bot =>
                bot.Value.Select(async queue =>
                {
                    var queueGrain = GrainFactory.GetGrain<IQueueGrain>($"{bot.Key}-{queue}");
                    await queueGrain.RegisterAttendantAsync(attendant.AttendantName, attendant.MaxSlots, attendant.CurrentTickets.Count());
                }));

            await Task.WhenAll(tasks);
        }

        public async Task GoOfflineAsync()
        {
            var tasks = AttendantData.BotsInformation.SelectMany(
                bot =>
                bot.Value.Select(async queue =>
                {
                    var queueGrain = GrainFactory.GetGrain<IQueueGrain>($"{bot}-{queue}");
                    await queueGrain.UnRegisterAttendantAsync(AttendantData.AttendantName);
                }));

            await Task.WhenAll(tasks);
        }

        public async Task AssignTicketAsync(Ticket ticket, DateTime currentDate)
        {
            AttendantData.CurrentTickets.Add(ticket);
            AttendantData.LastTicketReceivedDate = currentDate;
        }

        public async Task FinishTicketAsync(int ticketId)
        {
            var ticket = AttendantData.CurrentTickets.FirstOrDefault(t => t.Id == ticketId);

            if (ticket != null)
            {
                AttendantData.CurrentTickets.Remove(ticket);
            }
        }
    }
}
