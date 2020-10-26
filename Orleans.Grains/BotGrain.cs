using Orleans.Interfaces;
using System.Threading.Tasks;

namespace Orleans.Grains
{
    public class BotGrain : Grain, IBotGrain
    {
        public async Task FowardTicketAsync(Ticket ticket)
        {
            var queueGrain = GrainFactory.GetGrain<IQueueGrain>($"{ticket.BotIdentity}-{ticket.QueueName}");
            await queueGrain.AddToQueueAsync(ticket);
            await queueGrain.DistributeTicketAsync();
        }
    }
}
