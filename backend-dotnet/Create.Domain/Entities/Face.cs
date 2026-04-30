using System;

namespace Create.Domain.Entities
{
    public class Face
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public float[] Embedding { get; set; } = Array.Empty<float>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
