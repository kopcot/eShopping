using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Users.Core.Entities
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class IpConnection : BaseEntity
    {
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string IpAddress { get; set; }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
