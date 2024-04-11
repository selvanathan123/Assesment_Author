using Microsoft.EntityFrameworkCore;
using InspirinngQuotes.Models;

namespace InspirinngQuotes.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Quote> Quotes => Set<Quote>();
    }
}
