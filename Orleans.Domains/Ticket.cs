using System;

namespace Orleans.Interfaces
{
    public class Ticket
    {
        public Ticket(int id, string queueName, string botIdentity)
        {
            Id = id;
            QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            BotIdentity = botIdentity;
        }

        public int Id { get; }
        public string QueueName { get; set;  }
        public string BotIdentity { get; set; }
    }
}
