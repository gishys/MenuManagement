using MenuManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace MenuManagement.EntityFrameworkCore;

/// <summary>
/// 菜单管理数据库上下文
/// </summary>
[ConnectionStringName("Default")]
public class MenuManagementDbContext(DbContextOptions<MenuManagementDbContext> options) : AbpDbContext<MenuManagementDbContext>(options)
{
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuRole> MenuRoles { get; set; }
    public DbSet<MenuOrganization> MenuOrganizations { get; set; }

    // 共享的 DateTime 转换器实例（提高性能，避免重复创建）
    private static readonly ValueConverter<DateTime, DateTime> DateTimeUtcConverter =
        new(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> NullableDateTimeUtcConverter =
        new(
            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //builder.ConfigurePermissionManagement();
        //builder.ConfigureIdentity();

        // 配置所有 DateTime 属性使用 UTC（PostgreSQL timestamp with time zone 要求）
        // 使用共享的转换器实例以提高性能
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(DateTimeUtcConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(NullableDateTimeUtcConverter);
                }
            }
        }

        // 配置Menu实体
        builder.Entity<Menu>(b =>
        {
            b.ToTable("Menus");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.Code).IsRequired().HasMaxLength(100);
            b.Property(x => x.Path).HasMaxLength(200);
            b.Property(x => x.Component).HasMaxLength(200);
            b.Property(x => x.Icon).HasMaxLength(50);
            b.Property(x => x.Permission).HasMaxLength(100);
            b.Property(x => x.ExternalUrl).HasMaxLength(500);
            b.Property(x => x.Remark).HasMaxLength(500);

            b.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.Code).IsUnique();
        });

        // 配置MenuRole实体
        builder.Entity<MenuRole>(b =>
        {
            b.ToTable("MenuRoles");
            b.ConfigureByConvention();

            b.HasKey(x => new { x.MenuId, x.RoleId });

            b.HasOne(x => x.Menu)
                .WithMany(x => x.MenuRoles)
                .HasForeignKey(x => x.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // 添加查询过滤器，只返回关联的Menu未被软删除的记录
            b.HasQueryFilter(x => x.Menu != null && !x.Menu.IsDeleted);

            b.HasIndex(x => new { x.MenuId, x.RoleId }).IsUnique();
        });

        // 配置MenuOrganization实体
        builder.Entity<MenuOrganization>(b =>
        {
            b.ToTable("MenuOrganizations");
            b.ConfigureByConvention();

            b.HasKey(x => new { x.MenuId, x.OrganizationUnitId });

            b.HasOne(x => x.Menu)
                .WithMany(x => x.MenuOrganizations)
                .HasForeignKey(x => x.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // 添加查询过滤器，只返回关联的Menu未被软删除的记录
            b.HasQueryFilter(x => x.Menu != null && !x.Menu.IsDeleted);

            b.HasIndex(x => new { x.MenuId, x.OrganizationUnitId }).IsUnique();
        });
    }
}
