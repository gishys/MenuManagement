using MenuManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;

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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigurePermissionManagement();
        builder.ConfigureIdentity();

        // 配置Menu实体
        builder.Entity<Menu>(b =>
        {
            b.ToTable("Menus");
            b.ConfigureByConvention();

            b.Property(x => x.Name).IsRequired().HasMaxLength(50);
            b.Property(x => x.Code).IsRequired().HasMaxLength(50);
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

            b.HasIndex(x => new { x.MenuId, x.OrganizationUnitId }).IsUnique();
        });
    }
}
