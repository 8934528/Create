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
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly FaceService _faceService;
        private readonly ApplicationDbContext _db;
        private readonly FaceCacheService _cache;

        public AuthController(FaceService faceService, ApplicationDbContext db, FaceCacheService cache)
        {
            _faceService = faceService;
            _db = db;
            _cache = cache;
        }

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
