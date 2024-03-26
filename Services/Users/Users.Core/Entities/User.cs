using Microsoft.EntityFrameworkCore;         // for [Index] attribute
using Shared.Core.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; // for [Key] and [Required] attributes
using Users.Core.Enums;

namespace Users.Core.Entities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Index(nameof(Name), IsUnique = true)]
    public class User : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        public string? Email { get; set; }
        public UserRole.RoleType Role { get; set; }
        [Required]
        [PasswordPropertyText(true)]
        public string Password { get; set; }
        public uint Code { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
