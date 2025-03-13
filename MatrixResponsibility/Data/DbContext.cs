using Microsoft.EntityFrameworkCore;
using MatrixResponsibility.Common;

namespace MatrixResponsibility.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }
    }
}
