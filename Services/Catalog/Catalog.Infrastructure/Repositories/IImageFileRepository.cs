using Catalog.Core.Entities;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Repositories
{
    public interface IImageFileRepository : IAsyncRepository<ImageFileDirectory>
    {
    }
}
