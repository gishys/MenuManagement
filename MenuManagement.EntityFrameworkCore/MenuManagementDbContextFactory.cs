using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MenuManagement.EntityFrameworkCore;

/// <summary>
/// 数据库上下文工厂（用于EF Core迁移）
/// </summary>
public class MenuManagementDbContextFactory : IDesignTimeDbContextFactory<MenuManagementDbContext>
{
    public MenuManagementDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<MenuManagementDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Default"));

        return new MenuManagementDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
