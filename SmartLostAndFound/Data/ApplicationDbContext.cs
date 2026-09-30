using Microsoft.EntityFrameworkCore;
using SmartLostAndFound.Models;

namespace SmartLostAndFound.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LostFoundItem> LostFoundItems { get; set; }
    }
}