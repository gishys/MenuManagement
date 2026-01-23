using MenuManagement.Application.Contracts;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace MenuManagement.HttpApi;

/// <summary>
/// 菜单管理HTTP API模块
/// </summary>
[DependsOn(
    typeof(MenuManagementApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule)
)]
public class MenuManagementHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(MenuManagementApplicationContractsModule).Assembly);
        });
    }
}
