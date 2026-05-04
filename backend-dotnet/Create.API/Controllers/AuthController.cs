using Create.Application.DTOs;
using Create.Application.Services;
using Create.Domain.Entities;
using Create.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Create.API.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and registration with face embeddings.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly FaceService _faceService;
        private readonly ApplicationDbContext _db;
        private readonly IFaceCacheService _cache;

        public AuthController(FaceService faceService, ApplicationDbContext db, IFaceCacheService cache)
        {
            _faceService = faceService;
            _db = db;
            _cache = cache;
        }

        /// <summary>
        /// Registers a new user, hashes their password, and extracts/stores their facial embedding.
        /// </summary>
        /// <param name="dto">The registration data including user details and a face image.</param>
        /// <returns>A status result indicating success or failure of the registration process.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                // Check if user already exists
                if (_db.Users.Any(u => u.Email == dto.Email))
                {
                    return BadRequest("User with this email already exists");
                }

                // Create user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = dto.FullName,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = dto.Role
                };

                _db.Users.Add(user);

                var embedding = await _faceService.RegisterFace(dto.Image);

                if (embedding == null || embedding.Count == 0)
                {
                    return BadRequest("Failed to extract face embedding. Please ensure your face is visible.");
                }

                // Save embedding
                var face = new Face
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Embedding = embedding.ToArray()
                };

                _db.Faces.Add(face);
                await _db.SaveChangesAsync();

                // Reload cache
                await _cache.ReloadAsync();

                return Ok(new { message = "User registered successfully with face embedding", userId = user.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
