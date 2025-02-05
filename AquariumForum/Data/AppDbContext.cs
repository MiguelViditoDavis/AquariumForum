using Microsoft.EntityFrameworkCore;
using AquariumForum.Models; // Ensure correct namespace is used

namespace AquariumForum
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Corrected plural naming convention for DbSet
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<AquariumForum.Models.Comment> Comment { get; set; } = default!;
    }
}
