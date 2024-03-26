using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.Core.Entities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string? ImageFileName { get; set; }
        [ForeignKey("ImageFileDirectory")]
        public int? ImageFileID { get; set; }
        public virtual ImageFileDirectory? ImageFileDirectory { get; set; }
        [NotMapped]
        public string? ImageFilePath => ((ImageFileID.HasValue) && (ImageFileDirectory?.Directory.Length > 0) && (ImageFileName?.Length > 0)) ?
            Path.Combine(ImageFileDirectory?.Directory ?? string.Empty, ImageFileName) :
            ImageFileName;
        [ForeignKey("ProductBrand")]
        public int BrandID { get; set; }
        public virtual ProductBrand Brand { get; set; }
        [ForeignKey("ProductType")]
        public int TypeID { get; set; }
        public virtual ProductType Type { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
