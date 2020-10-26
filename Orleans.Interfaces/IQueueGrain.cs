using System.Threading.Tasks;

namespace Orleans.Interfaces
{
    public interface IQueueGrain : IGrainWithStringKey
    {
        Task AddToQueueAsync(Ticket ticket);
        Task RegisterAttendantAsync(string attendantName, int maxSlots, int inAttendance);
        Task UnRegisterAttendantAsync(string attendantName);
        Task DistributeTicketAsync();
    }
}
