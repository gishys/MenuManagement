using MenuManagement.Domain;
using MenuManagement.Domain.Entities;
using MenuManagement.Domain.Repositories;
using MenuManagement.EntityFrameworkCore.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;

namespace MenuManagement.EntityFrameworkCore;

/// <summary>
/// 菜单管理EF Core模块
/// </summary>
[DependsOn(
    typeof(AbpEntityFrameworkCoreModule),
    typeof(MenuManagementDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule)
)]
public class MenuManagementEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // 配置使用 PostgreSQL
        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });

        context.Services.AddAbpDbContext<MenuManagementDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);

            // 注册自定义仓储
            options.AddRepository<Menu, MenuRepository>();
        });
    }
}
