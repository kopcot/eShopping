using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Core.Entities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Index(nameof(Name), IsUnique = true)]
    public class ProductType : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        //public virtual List<Product>? Products { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
