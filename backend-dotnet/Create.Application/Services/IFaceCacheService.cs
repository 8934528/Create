using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Create.Application.Services
{
    public interface IFaceCacheService
    {
        Task LoadAsync();
        Task ReloadAsync();
        List<CachedFace> GetAll();
    }

    public class CachedFace
    {
        public Guid UserId { get; set; }
        public float[] Embedding { get; set; } = Array.Empty<float>();
    }
}
