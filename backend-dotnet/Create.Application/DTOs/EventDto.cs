using System;

namespace Create.Application.DTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Type { get; set; } = "ClassAttendance";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
