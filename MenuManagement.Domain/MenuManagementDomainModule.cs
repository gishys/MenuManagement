using Volo.Abp.Domain;
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
}
