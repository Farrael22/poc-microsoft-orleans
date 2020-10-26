using System;

namespace Orleans.Interfaces
{
    public class Ticket
    {
        public Ticket(int id, string queueName, string attendantIdentity, string botIdentity)
        {
            Id = id;
            QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            AttendantIdentity = attendantIdentity;
            BotIdentity = botIdentity;
        }

        public int Id { get; }
        public string QueueName { get; set;  }
        public string AttendantIdentity { get; set; }
        public string BotIdentity { get; set; }
    }
}
