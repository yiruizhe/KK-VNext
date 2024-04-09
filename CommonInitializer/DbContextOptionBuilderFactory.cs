using Microsoft.EntityFrameworkCore;

namespace CommonInitializer;

public static class DbContextOptionBuilderFactory
{
    public static DbContextOptionsBuilder<TDbContext> Create<TDbContext>() where TDbContext : DbContext
    {
        string? connStr = Environment.GetEnvironmentVariable("DefaultDB:ConnStr");
        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
        optionsBuilder.UseSqlServer(connStr);
        return optionsBuilder;
    }
}
