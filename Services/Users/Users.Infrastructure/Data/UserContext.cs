using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;
using Users.Core.Entities;
using Users.Infrastructure.Security;

namespace Users.Infrastructure.Data
{
    public class UserContext : BaseDbContext
    {
        public static DateTime StartTimeStamp { get; set; } = DateTime.UtcNow;
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().Property(e => e.Name)
                .HasConversion(
                    convertToProviderExpression: name => name.ToLowerInvariant(),
                    convertFromProviderExpression: name => name
                );
        }
    }
}
