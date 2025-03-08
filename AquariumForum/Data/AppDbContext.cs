using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AquariumForum.Models; // Ensure correct namespace is used

namespace AquariumForum
{
    // Inherit from IdentityDbContext to enable Identity authentication
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Corrected plural naming convention for DbSet
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<Comment> Comments { get; set; } = default!;

        // Ensure Identity model configurations are applied
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Calls IdentityDbContext's OnModelCreating method
        }
    }
}
