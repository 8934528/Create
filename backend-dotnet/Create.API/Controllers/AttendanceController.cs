using Create.Domain.Entities;
using Create.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Create.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AttendanceController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        [HttpPost("register-face")]
        [Authorize]
        public async Task<IActionResult> RegisterFace([FromBody] FaceRegistrationRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // 1. Send image to AI Service to get embedding
            var aiServiceUrl = _configuration["AiService:Url"] ?? "http://localhost:8000";
            var response = await _httpClient.PostAsJsonAsync($"{aiServiceUrl}/extract-embedding", new { image_base64 = request.ImageBase64 });

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("Failed to extract face embedding");
            }

            var result = await response.Content.ReadFromJsonAsync<EmbeddingResponse>();
            if (result == null) return BadRequest("Invalid response from AI service");

            // 2. Save embedding to DB
            var faceEmbedding = new FaceEmbedding
            {
                UserId = Guid.Parse(userId),
                Embedding = JsonSerializer.Serialize(result.Embedding)
            };

            _context.Faces.Add(faceEmbedding);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Face registered successfully" });
        }

        [HttpPost("verify-attendance")]
        public async Task<IActionResult> VerifyAttendance([FromBody] AttendanceVerificationRequest request)
        {
            // 1. Check anti-spoofing first
            var aiServiceUrl = _configuration["AiService:Url"] ?? "http://localhost:8000";
            var spoofResponse = await _httpClient.PostAsJsonAsync($"{aiServiceUrl}/check-spoof", new { image_base64 = request.ImageBase64 });
            
            if (spoofResponse.IsSuccessStatusCode)
            {
                var spoofResult = await spoofResponse.Content.ReadFromJsonAsync<SpoofResult>();
                if (spoofResult != null && !spoofResult.IsReal)
                {
                    return BadRequest(new { message = "Anti-spoofing check failed", detail = spoofResult.Message });
                }
            }

            // 2. Get embedding for the current face
            var embeddingResponse = await _httpClient.PostAsJsonAsync($"{aiServiceUrl}/extract-embedding", new { image_base64 = request.ImageBase64 });
            if (!embeddingResponse.IsSuccessStatusCode) return BadRequest("Could not detect face");

            var currentEmbeddingResult = await embeddingResponse.Content.ReadFromJsonAsync<EmbeddingResponse>();
            if (currentEmbeddingResult == null) return BadRequest();

            // 3. Find matching user in DB
            // (Simple linear search for now, could be optimized with pgvector)
            var allFaces = await _context.Faces.Include(f => f.User).ToListAsync();
            foreach (var face in allFaces)
            {
                var storedEmbedding = JsonSerializer.Deserialize<float[]>(face.Embedding);
                if (storedEmbedding != null && CosineSimilarity(currentEmbeddingResult.Embedding, storedEmbedding) > 0.85) // Threshold
                {
                    // Match found!
                    var attendance = new AttendanceRecord
                    {
                        UserId = face.UserId,
                        Type = request.Type ?? "CheckIn",
                        Timestamp = DateTime.UtcNow
                    };
                    _context.AttendanceRecords.Add(attendance);
                    await _context.SaveChangesAsync();

                    return Ok(new { message = $"Attendance marked for {face.User.FullName}", user = face.User.Username });
                }
            }

            return NotFound("Face not recognized");
        }

        private double CosineSimilarity(float[] V1, float[] V2)
        {
            double dot = 0.0, mag1 = 0.0, mag2 = 0.0;
            for (int i = 0; i < V1.Length; i++)
            {
                dot += V1[i] * V2[i];
                mag1 += Math.Pow(V1[i], 2);
                mag2 += Math.Pow(V2[i], 2);
            }
            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }
    }

    public class FaceRegistrationRequest { public string ImageBase64 { get; set; } = string.Empty; }
    public class AttendanceVerificationRequest { public string ImageBase64 { get; set; } = string.Empty; public string? Type { get; set; } }
    public class EmbeddingResponse { public float[] Embedding { get; set; } = Array.Empty<float>(); }
    public class SpoofResult { public bool IsReal { get; set; } public string Message { get; set; } = string.Empty; }
}
