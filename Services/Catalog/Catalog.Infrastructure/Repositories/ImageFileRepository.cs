using Catalog.Core.Entities;
using Catalog.Infrastructure.Data;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Repositories
{
    public class ImageFileRepository : BaseRepository<ImageFileDirectory>, IImageFileRepository
    {
        public ImageFileRepository(CatalogContext catalogContext) : base(catalogContext)
        {
        }
    }
}
