using Microsoft.EntityFrameworkCore;         // for [Index] attribute
using Shared.Core.Entities;
using System.ComponentModel.DataAnnotations; // for [Key] and [Required] attributes

namespace Catalog.Core.Entities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Index(nameof(Name), IsUnique = true)]
    public class ProductBrand : BaseEntity
    {
        public string Name { get; set; }
        //public virtual List<Product> Products { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
