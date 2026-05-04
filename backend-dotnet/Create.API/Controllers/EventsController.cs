using Create.Domain.Entities;
using Create.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Create.API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public EventsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET /api/events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _db.Events
                .OrderByDescending(e => e.StartTime)
                .ToListAsync();
            return Ok(events);
        }

        // POST /api/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Create.Application.DTOs.EventDto dto)
        {
            var newEvent = new Event
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Location = dto.Location,
                Type = Enum.TryParse<EventType>(dto.Type, out var type) ? type : EventType.ClassAttendance,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };
            
            _db.Events.Add(newEvent);
            await _db.SaveChangesAsync();
            return Ok(newEvent);
        }

        // GET /api/events/{id}/attendance
        [HttpGet("{id}/attendance")]
        public async Task<IActionResult> GetAttendance(Guid id)
        {
            var attendances = await _db.Attendance
                .Where(a => a.EventId == id)
                .Include(a => a.User)
                .OrderByDescending(a => a.CheckInTime)
                .Select(a => new
                {
                    a.Id,
                    a.UserId,
                    FullName = a.User != null ? a.User.FullName : "Unknown",
                    Email = a.User != null ? a.User.Email : "N/A",
                    a.CheckInTime,
                    a.Status
                })
                .ToListAsync();

            return Ok(attendances);
        }

        // GET /api/events/{id}/export-pdf
        [HttpGet("{id}/export-pdf")]
        public async Task<IActionResult> ExportPdf(Guid id)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound("Event not found");

            var attendances = await _db.Attendance
                .Where(a => a.EventId == id)
                .Include(a => a.User)
                .OrderBy(a => a.CheckInTime)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Inch);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Verdana));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Attendance Report").FontSize(24).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{ev.Name}").FontSize(16).SemiBold();
                            col.Item().Text($"{ev.StartTime:f} - {ev.EndTime:f}");
                        });

                        row.ConstantItem(100).Text($"Total: {attendances.Count}").FontSize(14).SemiBold().AlignRight();
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("#");
                            header.Cell().Element(CellStyle).Text("Full Name");
                            header.Cell().Element(CellStyle).Text("Email");
                            header.Cell().Element(CellStyle).Text("Time");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        for (int i = 0; i < attendances.Count; i++)
                        {
                            var a = attendances[i];
                            table.Cell().Element(CellStyle).Text($"{i + 1}");
                            table.Cell().Element(CellStyle).Text(a.User?.FullName ?? "Unknown");
                            table.Cell().Element(CellStyle).Text(a.User?.Email ?? "N/A");
                            table.Cell().Element(CellStyle).Text($"{a.CheckInTime:HH:mm:ss}");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            });

            var pdf = document.GeneratePdf();
            return File(pdf, "application/pdf", $"Attendance_{ev.Name.Replace(" ", "_")}.pdf");
        }
    }
}
