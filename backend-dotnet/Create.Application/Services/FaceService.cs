using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Create.Application.Services
{
    public class FaceService
    {
        private readonly HttpClient _http;

        public FaceService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<float>> RegisterFace(string base64Image)
        {
            var payload = JsonSerializer.Serialize(new { image = base64Image });

            var response = await _http.PostAsync(
                "http://localhost:8000/register/",
                new StringContent(payload, Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"AI Service returned error: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<FaceRegisterResponse>(json);

            if (result == null || result.embedding == null)
                return new List<float>();

            return result.embedding;
        }

        public async Task<VerifyResponse> VerifyFace(string base64Image, List<CachedFace> faces, string? prevImage = null)
        {
            var payload = JsonSerializer.Serialize(new
            {
                image = base64Image,
                prev_image = prevImage,
                users = faces.Select(f => new
                {
                    userId = f.UserId.ToString(),
                    embedding = f.Embedding
                })
            });

            var response = await _http.PostAsync(
                "http://localhost:8000/verify/",
                new StringContent(payload, Encoding.UTF8, "application/json")
            );

            if (!response.IsSuccessStatusCode)
            {
                return new VerifyResponse { match = false };
            }

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<VerifyResponse>(json);

            return result ?? new VerifyResponse { match = false };
        }
    }

    public class FaceRegisterResponse
    {
        public bool success { get; set; }
        public List<float> embedding { get; set; } = new();
    }

    public class VerifyResponse
    {
        public bool match { get; set; }
        public string? userId { get; set; }
        public string? error { get; set; }
    }
}
