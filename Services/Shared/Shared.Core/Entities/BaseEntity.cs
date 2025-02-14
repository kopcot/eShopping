using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Core.Entities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Index(nameof(Id), IsUnique = true)]
    public abstract class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
        [DefaultValue(false)]
        public bool IsHardDeleted { get; set; } = false;
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        [DefaultValue(0)]
        public uint? ModifiedCount { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
