using Create.Domain.Entities;
using Create.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Create.API.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                // Look for any events.
                if (context.Events.Any())
                {
                    return;   // DB has been seeded
                }

                var events = new Event[]
                {
                    new Event
                    {
                        Id = Guid.NewGuid(),
                        Name = "General Attendance",
                        Location = "Main Office",
                        StartTime = DateTime.UtcNow.Date.AddHours(8),
                        EndTime = DateTime.UtcNow.Date.AddHours(17)
                    },
                    new Event
                    {
                        Id = Guid.NewGuid(),
                        Name = "Weekly Team Sync",
                        Location = "Conference Room A",
                        StartTime = DateTime.UtcNow.Date.AddDays(1).AddHours(10),
                        EndTime = DateTime.UtcNow.Date.AddDays(1).AddHours(11)
                    }
                };

                foreach (var e in events)
                {
                    context.Events.Add(e);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
