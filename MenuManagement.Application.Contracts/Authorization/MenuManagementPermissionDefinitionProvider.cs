using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MenuManagement.Application.Contracts.Authorization;

/// <summary>
/// 菜单管理权限定义提供者
/// </summary>
public class MenuManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var menuManagementGroup = context.AddGroup("MenuManagement", L("Permission:MenuManagement"));

        // 菜单权限
        var menuPermission = menuManagementGroup.AddPermission("MenuManagement.Menus", L("Permission:Menus"));
        menuPermission.AddChild("MenuManagement.Menus.Create", L("Permission:Menus.Create"));
        menuPermission.AddChild("MenuManagement.Menus.Update", L("Permission:Menus.Update"));
        menuPermission.AddChild("MenuManagement.Menus.Delete", L("Permission:Menus.Delete"));
        menuPermission.AddChild("MenuManagement.Menus.View", L("Permission:Menus.View"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MenuManagementResource>(name);
    }
}

/// <summary>
/// 菜单管理资源（用于本地化）
/// </summary>
public class MenuManagementResource
{
}
