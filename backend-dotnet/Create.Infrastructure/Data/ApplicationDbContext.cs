using Create.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Create.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Face> Faces { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Attendance> Attendance { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.HasPostgresExtension("vector");

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(150);
                entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(150);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
                entity.Property(e => e.Role).HasColumnName("role").IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Face>(entity =>
            {
                entity.ToTable("faces");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                
                var vectorConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<float[], Pgvector.Vector>(
                    v => new Pgvector.Vector(v),
                    v => v.ToArray()
                );

                entity.Property(e => e.Embedding)
                      .HasColumnName("embedding")
                      .HasColumnType("vector(128)")
                      .HasConversion(vectorConverter);

                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Faces)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(150);
                entity.Property(e => e.Location).HasColumnName("location").HasMaxLength(150);
                entity.Property(e => e.StartTime).HasColumnName("start_time");
                entity.Property(e => e.EndTime).HasColumnName("end_time");
                entity.Property(e => e.Type).HasColumnName("event_type").HasConversion<string>();
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.ToTable("attendance");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").HasDefaultValueSql("uuid_generate_v4()");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.EventId).HasColumnName("event_id");
                entity.Property(e => e.CheckInTime).HasColumnName("check_in_time").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Attendances)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
