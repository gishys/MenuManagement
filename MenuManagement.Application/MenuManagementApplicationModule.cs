using MenuManagement.Application.Contracts;
using MenuManagement.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.Authorization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace MenuManagement.Application;

/// <summary>
/// 菜单管理应用模块
/// </summary>
[DependsOn(
    typeof(MenuManagementApplicationContractsModule),
    typeof(MenuManagementDomainModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementDomainModule)
)]
public class MenuManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<MenuManagementApplicationModule>();
        });
    }
}
