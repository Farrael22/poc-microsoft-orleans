using System;
using System.Threading.Tasks;

namespace Orleans.Interfaces
{
    public interface IAttendantGrain : IGrainWithStringKey
    {
        Task GoOnlineAsync(Attendant attendant);
        Task GoOfflineAsync();
        Task AssignTicketAsync(Ticket ticket, DateTime currentDate);
        Task FinishTicketAsync(int ticketId);
    }
}