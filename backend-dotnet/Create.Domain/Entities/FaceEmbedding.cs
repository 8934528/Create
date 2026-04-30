using System;

namespace Create.Domain.Entities
{
    public class FaceEmbedding
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Embedding { get; set; } = string.Empty; // Store as JSON string or float array
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
