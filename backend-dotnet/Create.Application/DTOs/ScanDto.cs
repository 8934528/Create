using System;

namespace Create.Application.DTOs
{
    public class ScanDto
    {
        public string Image { get; set; } = string.Empty;   // base64 current frame
        public string? PrevImage { get; set; }  // base64 
        public Guid EventId { get; set; }
    }
}
