using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MenuManagement.Application.Contracts.Authorization;

/// <summary>
/// 菜单管理权限定义提供者。
/// 本地化使用宿主应用的 "Geo" 资源（Hx.Abp.Geo.Application/Localization/Geo/zh-Hans.json），
/// 其中已包含所有 Permission:MenuManagement / Permission:Menus.* 的中文翻译。
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

    /// <summary>
    /// 使用宿主 "Geo" 本地化资源，避免为单个包单独维护 zh-Hans.json。
    /// 所有 Permission:Menus.* 翻译条目已在 Geo/zh-Hans.json 中定义。
    /// </summary>
    private static LocalizableString L(string name) => new(name, "Geo");
}

/// <summary>
/// 菜单管理本地化资源标记类（保留以兼容已有代码中的类型引用）。
/// </summary>
public class MenuManagementResource
{
}
