using System;

namespace Orleans.Interfaces
{
    public class AttendantRecord
    {
        public AttendantRecord(string name, int maxSlots, int inAttendance = 0)
        {
            Identity = name ?? throw new ArgumentNullException(nameof(name));
            MaxSlots = maxSlots;
            InAttendance = inAttendance;
        }

        public string Identity { get; }
        public int MaxSlots { get; set; }
        public int InAttendance { get; set; }
        public DateTime LastTicketReceivedDate { get; set; }

        public int AvailableSlots => MaxSlots - InAttendance;
        public bool IsAvailable => InAttendance < MaxSlots;
    }
}
