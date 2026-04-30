using System;

namespace Create.Domain.Entities
{
    public class AttendanceRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Type { get; set; } = "CheckIn"; // CheckIn, CheckOut
        public string? Location { get; set; }
    }
}
