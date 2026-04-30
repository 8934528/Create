using System;
using System.Collections.Generic;

namespace Create.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // student, employee, attendee
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Face> Faces { get; set; } = new List<Face>();
    }
}
