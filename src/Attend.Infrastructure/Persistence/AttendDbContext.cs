using Microsoft.EntityFrameworkCore;
using Attend.Domain.Entities;

namespace Attend.Infrastructure.Persistence;

public class AttendDbContext(DbContextOptions<AttendDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttendDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
