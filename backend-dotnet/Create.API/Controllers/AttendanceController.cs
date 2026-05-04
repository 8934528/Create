using Create.Application.DTOs;
using Create.Application.Services;
using Create.API.Hubs;
using Create.Domain.Entities;
using Create.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Create.API.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly FaceService _faceService;
        private readonly IFaceCacheService _cache;
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<AttendanceHub> _hub;

        public AttendanceController(
            FaceService faceService,
            IFaceCacheService cache,
            ApplicationDbContext db,
            IHubContext<AttendanceHub> hub)
        {
            _faceService = faceService;
            _cache = cache;
            _db = db;
            _hub = hub;
        }

        // POST /api/attendance/scan
        [HttpPost("scan")]
        public async Task<IActionResult> Scan([FromBody] ScanDto dto)
        {
            try
            {
                var faces = _cache.GetAll();

                if (faces.Count == 0)
                {
                    await _hub.Clients.All.SendAsync("ReceiveStatus",
                        "No registered faces in system.");
                    return Ok(new { success = false, message = "No registered faces in system" });
                }

                var result = await _faceService.VerifyFace(dto.Image, faces, dto.PrevImage);

                if (!result.match || string.IsNullOrEmpty(result.userId))
                {
                    var msg = result.error ?? "Face not recognized";
                    await _hub.Clients.All.SendAsync("ReceiveStatus", $"[Warning] {msg}");
                    return Ok(new { success = false, message = msg });
                }

                if (!Guid.TryParse(result.userId, out var userId))
                {
                    await _hub.Clients.All.SendAsync("ReceiveStatus",
                        "Invalid user ID from AI service.");
                    return Ok(new { success = false, message = "Invalid user ID from AI service" });
                }

                var today = DateTime.UtcNow.Date;
                var alreadyCheckedIn = await _db.Attendance.AnyAsync(a =>
                    a.UserId == userId &&
                    a.EventId == dto.EventId &&
                    a.CheckInTime.Date == today
                );

                if (alreadyCheckedIn)
                {
                    await _hub.Clients.All.SendAsync("ReceiveStatus",
                        "[Info] Already checked in for this event today.");
                    return Ok(new { success = false, message = "Already checked in for this event today" });
                }

                // Log attendance
                var attendance = new Attendance
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    EventId = dto.EventId,
                    Status = "present",
                    CheckInTime = DateTime.UtcNow
                };

                _db.Attendance.Add(attendance);
                await _db.SaveChangesAsync();

                // Fetch user name 
                var user = await _db.Users.FindAsync(userId);
                var fullName = user?.FullName ?? "Unknown";

                await _hub.Clients.All.SendAsync("ReceiveStatus",
                    $"[Success] Welcome, {fullName}. Attendance recorded.");

                return Ok(new
                {
                    success = true,
                    userId = userId,
                    fullName = fullName,
                    message = "Attendance marked successfully"
                });
            }
            catch (Exception ex)
            {
                await _hub.Clients.All.SendAsync("ReceiveStatus",
                    $"Server error: {ex.Message}");
                return StatusCode(500, $"Error processing scan: {ex.Message}");
            }
        }

        private async Task<Guid?> FindNearestUserAsync(float[] embedding, float maxDistance = 0.6f)
        {
            var vectorLiteral = "[" + string.Join(",",
                embedding.Select(v => v.ToString("G", System.Globalization.CultureInfo.InvariantCulture))) + "]";

            var sql = $@"
                SELECT id, user_id, embedding, created_at
                FROM faces
                ORDER BY embedding <-> '{vectorLiteral}'::vector
                LIMIT 1";

            var match = await _db.Faces
                .FromSqlRaw(sql)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (match == null) return null;

            var distance = EuclideanDistance(embedding, match.Embedding);
            return distance <= maxDistance ? match.UserId : null;
        }

        private static float EuclideanDistance(float[] a, float[] b)
        {
            if (a.Length != b.Length) return float.MaxValue;
            float sum = 0f;
            for (int i = 0; i < a.Length; i++) { float d = a[i] - b[i]; sum += d * d; }
            return MathF.Sqrt(sum);
        }

        // GET /api/attendance/report  — Admin dashboard analytics
        [HttpGet("report")]
        public async Task<IActionResult> GetReport()
        {
            var totalUsers = await _db.Users.CountAsync();
            var totalAttendance = await _db.Attendance.CountAsync();

            var today = DateTime.UtcNow.Date;
            var todayAttendance = await _db.Attendance
                .Where(a => a.CheckInTime.Date == today)
                .CountAsync();

            // Last 7 days trend
            var since = today.AddDays(-6);
            var last7DaysRaw = await _db.Attendance
                .Where(a => a.CheckInTime >= since)
                .Select(a => a.CheckInTime.Date)
                .ToListAsync();

            var last7Days = last7DaysRaw
                .GroupBy(d => d)
                .Select(g => new
                {
                    date = g.Key.ToString("yyyy-MM-dd"),
                    count = g.Count()
                })
                .OrderBy(x => x.date)
                .ToList();

            var fullWeek = Enumerable.Range(0, 7)
                .Select(i => today.AddDays(-6 + i).ToString("yyyy-MM-dd"))
                .Select(d => new
                {
                    date = d,
                    count = last7Days.FirstOrDefault(x => x.date == d)?.count ?? 0
                })
                .ToList();

            return Ok(new
            {
                totalUsers,
                totalAttendance,
                todayAttendance,
                last7Days = fullWeek
            });
        }
    }
}
