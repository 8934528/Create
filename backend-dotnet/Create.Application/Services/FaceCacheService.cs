using Create.Domain.Entities;
using Create.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Create.Application.Services
{
    public class FaceCacheService
    {
        private readonly IServiceProvider _serviceProvider;
        private List<CachedFace> _faces = new();
        private readonly object _lock = new();

        public FaceCacheService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task LoadAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var faces = await db.Faces
                .Select(f => new CachedFace
                {
                    UserId = f.UserId,
                    Embedding = f.Embedding
                })
                .ToListAsync();

            lock (_lock)
            {
                _faces = faces;
            }
        }

        public async Task ReloadAsync() => await LoadAsync();

        public List<CachedFace> GetAll()
        {
            lock (_lock)
            {
                return new List<CachedFace>(_faces);
            }
        }
    }

    public class CachedFace
    {
        public Guid UserId { get; set; }
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
