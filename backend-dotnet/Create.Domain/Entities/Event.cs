using System;
using System.Collections.Generic;

namespace Create.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
