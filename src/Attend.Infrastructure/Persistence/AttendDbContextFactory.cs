using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Attend.Infrastructure.Persistence;

public class AttendDbContextFactory : IDesignTimeDbContextFactory<AttendDbContext>
{
    public AttendDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AttendDbContext>();
        
        // Use default connection for migrations
        // Individual tenant databases will be created at runtime
        optionsBuilder.UseSqlite("Data Source=AttendDb.db");

        return new AttendDbContext(optionsBuilder.Options);
    }
}
