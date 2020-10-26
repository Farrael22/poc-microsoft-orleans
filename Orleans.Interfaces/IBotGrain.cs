using Orleans;
using System.Threading.Tasks;

namespace Orleans.Interfaces
{
    public interface IBotGrain : IGrainWithStringKey
    {
        Task FowardTicketAsync(Ticket ticket);
    }
}
