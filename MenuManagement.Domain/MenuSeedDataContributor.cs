using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MenuManagement.Domain.Entities;
using MenuManagement.Domain.Repositories;
using MenuManagement.Domain.Shared.Enums;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;

namespace MenuManagement.Domain;

/// <summary>
/// 菜单种子数据（与前端 menu/config 对齐）。
/// 后端无 key 概念，仅有 Code、Path；目录节点不设 Path，前端转换时 key=Id，避免与子项 path 重复。
/// 所有菜单项均配置 Icon，保证前端完全由后端 icon 字段驱动，无需静态兜底。
/// </summary>
public class MenuSeedDataContributor(
    IMenuRepository menuRepository,
    IIdentityRoleRepository roleRepository) : IDataSeedContributor, ITransientDependency
{
    private readonly IMenuRepository _menuRepository = menuRepository;
    private readonly IIdentityRoleRepository _roleRepository = roleRepository;

    /// <summary>
    /// admin 角色的 NormalizedName（ABP 默认管理员角色）
    /// </summary>
    private const string AdminRoleNormalizedName = "ADMIN";

    public async Task SeedAsync(DataSeedContext context)
    {
        // 约定：种子数据需要幂等（可重复执行、只补缺失项，不覆盖人工配置）

        // 地图服务管理（与前端 systemManagementMenu 首项一致）
        var mapServiceManagement = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "地图服务管理",
            code: "system-management",
            type: MenuType.Menu,
            parentId: null)
        {
            Path = "/system-management",
            Sort = 10,
            Status = MenuStatus.Enabled,
            Permission = "SystemManagement.MapService",
            Icon = "CompassOutlined"
        });

        // 身份管理（目录，顶级与前端一致）
        var identityManagement = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "身份管理",
            code: "identity-management",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 20,
            Icon = "UserOutlined",
            Status = MenuStatus.Enabled
        });

        // 身份管理子菜单
        var identityChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("用户管理", "identity-management:user", "/identity-management/user-management", 1, identityManagement.Id, "Identity.Users", "TeamOutlined")),
            await EnsureMenuAsync(CreateMenu("角色管理", "identity-management:role", "/identity-management/role-management", 2, identityManagement.Id, "Identity.Roles", "SafetyCertificateOutlined")),
            await EnsureMenuAsync(CreateMenu("组织管理", "identity-management:organization", "/identity-management/organization-management", 3, identityManagement.Id, "Identity.OrganizationUnits", "ApartmentOutlined")),
            await EnsureMenuAsync(CreateMenu("权限管理", "identity-management:permission", "/identity-management/permission-management", 4, identityManagement.Id, "Identity.Roles", "KeyOutlined")),
            await EnsureMenuAsync(CreateMenu("关联管理", "identity-management:association", "/identity-management/association-management", 5, identityManagement.Id, "Identity.OrganizationUnits", "LinkOutlined"))
        };

        // 菜单管理
        var menuManagement = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "菜单管理",
            code: "menu-management",
            type: MenuType.Menu,
            parentId: null)
        {
            Path = "/menu-management",
            Sort = 30,
            Status = MenuStatus.Enabled,
            Permission = "SystemManagement.MenuManagement",
            Icon = "MenuOutlined"
        });

        // 消息中心
        var messageCenter = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "消息中心",
            code: "message-center",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 40,
            Icon = "MessageOutlined",
            Status = MenuStatus.Enabled
        });

        var messageCenterChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("消息列表", "message-center:message-list", "/message-center/message-list", 1, messageCenter.Id, "SystemManagement.MessageCenter", "UnorderedListOutlined")),
            await EnsureMenuAsync(CreateMenu("消息模板管理", "message-center:template-management", "/message-center/template-management", 2, messageCenter.Id, "SystemManagement.MessageCenter", "FileTextOutlined"))
        };

        // 日志与行为管理
        var logsManagement = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "日志与行为管理",
            code: "logs-management",
            type: MenuType.Menu,
            parentId: null)
        {
            Path = "/logs-management",
            Sort = 45,
            Status = MenuStatus.Enabled,
            Permission = "Geo.Logs.View",
            Icon = "HistoryOutlined"
        });

        // 资源仓库
        var resourceWarehouse = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "资源仓库",
            code: "resource-warehouse",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 50,
            Icon = "DatabaseOutlined",
            Status = MenuStatus.Enabled
        });

        var resourceWarehouseChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("二维服务", "resource-warehouse:two-dimensional-service", "/resource-warehouse/two-dimensional-service", 1, resourceWarehouse.Id, "SystemManagement.TwoDimensionalService", "EnvironmentOutlined")),
            await EnsureMenuAsync(CreateMenu("资源编目", "resource-warehouse:resource-catalog", "/resource-warehouse/resource-catalog", 2, resourceWarehouse.Id, "SystemManagement.ResourceCatalog", "BookOutlined")),
            await EnsureMenuAsync(CreateMenu("组织授权", "resource-warehouse:organization-authorization", "/resource-warehouse/organization-authorization", 3, resourceWarehouse.Id, "SystemManagement.OrganizationAuthorization", "AuditOutlined")),
            await EnsureMenuAsync(CreateMenu("数据源管理", "resource-warehouse:datasource-management", "/resource-warehouse/datasource-management", 4, resourceWarehouse.Id, "SystemManagement.Datasource", "HddOutlined")),
            await EnsureMenuAsync(CreateMenu("地理模型管理", "resource-warehouse:geo-model-management", "/resource-warehouse/geo-model-management", 5, resourceWarehouse.Id, "SystemManagement.GeoModelManagement", "NodeIndexOutlined")),
            await EnsureMenuAsync(CreateMenu("地理模型参数模板管理", "resource-warehouse:geo-model-parameter-template-management", "/resource-warehouse/geo-model-parameter-template-management", 6, resourceWarehouse.Id, "SystemManagement.GeoModelParameterTemplate", "SettingOutlined")),
            await EnsureMenuAsync(CreateMenu("矢量服务接口配置", "resource-warehouse:vector-service-interface-management", "/resource-warehouse/vector-service-interface-management", 7, resourceWarehouse.Id, "SystemManagement.VectorServiceInterface", "ApiOutlined")),
            await EnsureMenuAsync(CreateMenu("专题分析配置", "resource-warehouse:thematic-analysis-config-management", "/resource-warehouse/thematic-analysis-config-management", 8, resourceWarehouse.Id, "SystemManagement.GeoModelManagement", "PieChartOutlined")),
            await EnsureMenuAsync(CreateMenu("实体类型管理", "resource-warehouse:entity-type-management", "/resource-warehouse/entity-type-management", 9, resourceWarehouse.Id, "SystemManagement.EntityTypeManagement", "TagOutlined")),
            await EnsureMenuAsync(CreateMenu("关系类型管理", "resource-warehouse:relation-type-management", "/resource-warehouse/relation-type-management", 10, resourceWarehouse.Id, "SystemManagement.RelationTypeManagement", "ShareAltOutlined")),
            await EnsureMenuAsync(CreateMenu("专题统计分析配置", "resource-warehouse:analysis-statistics-config-management", "/resource-warehouse/analysis-statistics-config-management", 11, resourceWarehouse.Id, "SystemManagement.GeoModelManagement", "BarChartOutlined")),
            await EnsureMenuAsync(CreateMenu("异步任务管理", "resource-warehouse:task-management", "/resource-warehouse/task-management", 12, resourceWarehouse.Id, "SystemManagement.TaskManagement", "ClockCircleOutlined")),
            await EnsureMenuAsync(CreateMenu("文件管理", "resource-warehouse:file-management", "/resource-warehouse/file-management", 13, resourceWarehouse.Id, "SystemManagement.FileManagement", "FileOutlined")),
            await EnsureMenuAsync(CreateMenu("地理模型执行管理", "resource-warehouse:geo-model-execution-management", "/resource-warehouse/geo-model-execution-management", 14, resourceWarehouse.Id, "SystemManagement.GeoModelExecutionManagement", "PlayCircleOutlined"))
        };

        // 资源管理（目录）
        var resourceManagement = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "资源管理",
            code: "resource-management",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 60,
            Icon = "FolderOutlined",
            Status = MenuStatus.Enabled
        });

        var resourceManagementChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("资源访问管理", "resource-management:resource-access-management", "/resource-management", 0, resourceManagement.Id, "ResourceManagement.ResourceAccessManagement", "SafetyOutlined")),
            await EnsureMenuAsync(CreateMenu("权限管理", "resource-management:permission-list", "/resource-management/permission", 1, resourceManagement.Id, "ResourceManagement.PermissionList", "KeyOutlined")),
            await EnsureMenuAsync(CreateMenu("审核历史", "resource-management:audit-history", "/resource-management/audit/history", 2, resourceManagement.Id, "ResourceManagement.AuditHistory", "HistoryOutlined")),
            await EnsureMenuAsync(CreateMenu("审核人管理", "resource-management:auditor-list", "/resource-management/auditor", 3, resourceManagement.Id, "ResourceManagement.AuditorList", "TeamOutlined"))
        };

        // 数据采集（目录）
        var dataCollection = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "数据采集",
            code: "data-collection",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 55,
            Icon = "DatabaseOutlined",
            Status = MenuStatus.Enabled
        });

        var dataCollectionChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("数据采集工作台", "data-collection:workbench", "/data-collection", 1, dataCollection.Id, "SystemManagement.DataCollection", "AppstoreOutlined")),
            await EnsureMenuAsync(CreateMenu("字典管理", "data-collection:dictionary", "/data-collection/dictionary", 2, dataCollection.Id, "SystemManagement.DictionaryManagement", "BookOutlined")),
            await EnsureMenuAsync(CreateMenu("模板与规则管理", "data-collection:template", "/data-collection/template", 3, dataCollection.Id, "SystemManagement.DataGovernanceAdmin", "ProfileOutlined"))
        };

        // 默认模块简易菜单
        var home = await EnsureMenuAsync(CreateMenu("首页", "home", "/", 1, null, null, "HomeOutlined"));
        var oneMap = await EnsureMenuAsync(CreateMenu("一张图", "page", "/page", 2, null, null, "GlobalOutlined"));

        // 为 admin 角色赋予所有菜单权限
        var adminRole = await _roleRepository.FindByNormalizedNameAsync(AdminRoleNormalizedName);
        if (adminRole != null)
        {
            var existing = await _menuRepository.GetMenusByRoleIdAsync(adminRole.Id);
            var assigned = existing.Select(m => m.Id).ToHashSet();

            var shouldAssign = new List<Guid>
            {
                mapServiceManagement.Id,
                identityManagement.Id,
                messageCenter.Id,
                logsManagement.Id,
                resourceWarehouse.Id,
                resourceManagement.Id,
                dataCollection.Id,
                menuManagement.Id,
                home.Id,
                oneMap.Id
            };
            shouldAssign.AddRange(identityChildren.Select(m => m.Id));
            shouldAssign.AddRange(messageCenterChildren.Select(m => m.Id));
            shouldAssign.AddRange(resourceWarehouseChildren.Select(m => m.Id));
            shouldAssign.AddRange(resourceManagementChildren.Select(m => m.Id));
            shouldAssign.AddRange(dataCollectionChildren.Select(m => m.Id));

            var merged = assigned.Union(shouldAssign).Distinct().ToList();
            await _menuRepository.ReplaceMenusForRoleAsync(adminRole.Id, merged);
        }
    }

    /// <param name="permission">ABP 权限名（与主后端 MenuPermissionNames 一致）；为 null 时表示无权限要求（如首页、一张图）</param>
    /// <param name="icon">Ant Design 图标组件名（与前端菜单一致）；使用字符串形式，如 "HomeOutlined"</param>
    private static Menu CreateMenu(
        string name,
        string code,
        string path,
        int sort,
        Guid? parentId = null,
        string? permission = null,
        string? icon = null)
    {
        return new Menu(
            id: Guid.NewGuid(),
            name: name,
            code: code,
            type: MenuType.Menu,
            parentId: parentId)
        {
            Path = path,
            Sort = sort,
            Status = MenuStatus.Enabled,
            Permission = permission,
            Icon = icon
        };
    }

    private async Task<Menu> EnsureMenuAsync(Menu menu)
    {
        var existing = await _menuRepository.GetByCodeAsync(menu.Code);
        if (existing == null)
        {
            await _menuRepository.InsertAsync(menu, autoSave: true);
            return menu;
        }

        // 仅补齐关键字段（不覆盖人工调整的 Name/Sort 等），避免破坏线上配置
        var changed = false;
        if (existing.Type != menu.Type) { existing.Type = menu.Type; changed = true; }
        if (existing.ParentId != menu.ParentId) { existing.ParentId = menu.ParentId; changed = true; }
        if (existing.Path == null && menu.Path != null) { existing.Path = menu.Path; changed = true; }
        if (string.IsNullOrWhiteSpace(existing.Permission) && !string.IsNullOrWhiteSpace(menu.Permission)) { existing.Permission = menu.Permission; changed = true; }
        // 始终用种子数据的 Icon 覆盖，确保已有环境能同步最新图标配置
        if (existing.Icon != menu.Icon && menu.Icon != null) { existing.Icon = menu.Icon; changed = true; }
        if (existing.Status != MenuStatus.Enabled) { existing.Status = MenuStatus.Enabled; changed = true; }
        if (changed)
        {
            await _menuRepository.UpdateAsync(existing, autoSave: true);
        }
        return existing;
    }
}
