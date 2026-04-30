using System;

namespace Create.Domain.Entities
{
    public class Attendance
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid? EventId { get; set; }
        public Event? Event { get; set; }
        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "present"; // present, late, denied
    }
}
