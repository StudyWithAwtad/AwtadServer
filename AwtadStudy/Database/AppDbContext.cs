using AwtadStudy.Universities;
using AwtadStudy.Users;
using Microsoft.EntityFrameworkCore;

namespace AwtadStudy.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<University>();

        modelBuilder.Entity<User>()
            .OwnsMany(u => u.Courses, c =>
            {
                c.ToJson();
            });
    }
}
