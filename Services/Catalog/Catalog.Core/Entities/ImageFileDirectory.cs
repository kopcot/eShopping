using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;
using System.Xml.Linq;

namespace Catalog.Core.Entities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Index(nameof(Directory), IsUnique = true)]
    public class ImageFileDirectory : BaseEntity
    {
        public string Directory { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
