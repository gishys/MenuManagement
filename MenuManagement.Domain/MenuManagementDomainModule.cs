using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Domain;
using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace MenuManagement.Domain;

/// <summary>
/// 菜单管理领域模块
/// </summary>
[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(Domain.Shared.MenuManagementDomainSharedModule)
)]
public class MenuManagementDomainModule : AbpModule
{
    public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        // 应用启动时执行菜单种子数据（仅首次，无数据时）
        var dataSeeder = context.ServiceProvider.GetRequiredService<IDataSeeder>();
        await dataSeeder.SeedAsync();
    }
}
