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

        // GIS 服务管理（目录：聚合地图总览 + GIS 服务配置；对应权限组 GisService）
        var mapServiceManagement = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "GIS 服务管理",
            code: "system-management",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 10,
            Status = MenuStatus.Enabled,
            Icon = "CompassOutlined"
        });

        var mapServiceChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("地图服务总览", "system-management:overview", "/system-management", 1, mapServiceManagement.Id, "GisService.MapService", "CompassOutlined")),
            await EnsureMenuAsync(CreateMenu("二维服务", "system-management:two-dimensional-service", "/system-management/two-dimensional-service", 2, mapServiceManagement.Id, "GisService.TwoDimensionalService", "EnvironmentOutlined")),
            await EnsureMenuAsync(CreateMenu("矢量服务接口配置", "system-management:vector-service-interface", "/system-management/vector-service-interface", 3, mapServiceManagement.Id, "GisService.VectorServiceInterface", "ApiOutlined")),
            await EnsureMenuAsync(CreateMenu("GeoServer 对齐管理", "system-management:geoserver-align-management", "/system-management/geoserver-align-management", 4, mapServiceManagement.Id, "GisService.TwoDimensionalService", "SyncOutlined"))
        };

        // 三维服务（目录：三维场景工作台；对应权限组 ThreeDScene）
        var threeDService = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "三维服务",
            code: "three-d-service",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 15,
            Status = MenuStatus.Enabled,
            Icon = "GlobalOutlined"
        });

        var threeDServiceChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("三维场景", "three-d-service:scene", "/3d-scene", 1, threeDService.Id, "ThreeDScene.ModelManagement", "GlobalOutlined"))
        };

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
            await EnsureMenuAsync(CreateMenu("用户管理", "identity-management:user", "/identity-management/user-management", 1, identityManagement.Id, "AbpIdentity.Users", "TeamOutlined")),
            await EnsureMenuAsync(CreateMenu("角色管理", "identity-management:role", "/identity-management/role-management", 2, identityManagement.Id, "AbpIdentity.Roles", "SafetyCertificateOutlined")),
            await EnsureMenuAsync(CreateMenu("组织管理", "identity-management:organization", "/identity-management/organization-management", 3, identityManagement.Id, "AbpIdentity.OrganizationUnits", "ApartmentOutlined")),
            await EnsureMenuAsync(CreateMenu("权限管理", "identity-management:permission", "/identity-management/permission-management", 4, identityManagement.Id, "AbpIdentity.Roles", "KeyOutlined")),
            await EnsureMenuAsync(CreateMenu("关联管理", "identity-management:association", "/identity-management/association-management", 5, identityManagement.Id, "AbpIdentity.OrganizationUnits", "LinkOutlined"))
        };

        // 菜单管理（使用 MenuManagement.Menus 权限，与前端 systemManagementMenu.ts 及路由守卫保持一致）
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
            Permission = "MenuManagement.Menus",
            Icon = "MenuOutlined"
        });

        // 消息通知（对应权限组 MessageCenter；与 Permission:MessageCenter = "消息通知" 对齐）
        var messageCenter = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "消息通知",
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
            await EnsureMenuAsync(CreateMenu("消息列表", "message-center:message-list", "/message-center/message-list", 1, messageCenter.Id, "MessageCenter.Messages", "UnorderedListOutlined")),
            await EnsureMenuAsync(CreateMenu("消息模板管理", "message-center:template-management", "/message-center/template-management", 2, messageCenter.Id, "MessageCenter.Templates", "FileTextOutlined"))
        };

        // 运维监控（目录：行为日志 + 操作审计日志；对应权限组 Monitoring = "运维监控"）
        var logsManagement = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "运维监控",
            code: "logs-management",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 50,
            Status = MenuStatus.Enabled,
            Icon = "HistoryOutlined"
        });

        var logsManagementChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("行为日志", "logs-management:behavior", "/logs-management", 1, logsManagement.Id, "Monitoring.BehaviorLog", "HistoryOutlined")),
            await EnsureMenuAsync(CreateMenu("操作审计日志", "logs-management:audit", "/logs-management/audit", 2, logsManagement.Id, "Monitoring.AuditLog", "AuditOutlined"))
        };

        // 空间数据仓库（对应权限组 DataWarehouse = "空间数据仓库"）
        var resourceWarehouse = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "空间数据仓库",
            code: "resource-warehouse",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 60,
            Icon = "DatabaseOutlined",
            Status = MenuStatus.Enabled
        });

        // 资源仓库核心数据资产管理（GIS服务类已移至"地图服务管理"，地理模型已独立，分析配置已独立）
        var resourceWarehouseChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("资源编目", "resource-warehouse:resource-catalog", "/resource-warehouse/resource-catalog", 1, resourceWarehouse.Id, "DataWarehouse.ResourceCatalog", "BookOutlined")),
            await EnsureMenuAsync(CreateMenu("数据源管理", "resource-warehouse:datasource-management", "/resource-warehouse/datasource-management", 2, resourceWarehouse.Id, "DataWarehouse.Datasource", "HddOutlined")),
            await EnsureMenuAsync(CreateMenu("组织授权", "resource-warehouse:organization-authorization", "/resource-warehouse/organization-authorization", 3, resourceWarehouse.Id, "DataWarehouse.OrganizationAuth", "AuditOutlined")),
            await EnsureMenuAsync(CreateMenu("SDE同步管理", "resource-warehouse:sde-sync-management", "/resource-warehouse/sde-sync-management", 4, resourceWarehouse.Id, "DataWarehouse.SdeSync", "SyncOutlined")),
            await EnsureMenuAsync(CreateMenu("文件管理", "resource-warehouse:file-management", "/resource-warehouse/file-management", 5, resourceWarehouse.Id, "DataWarehouse.FileManagement", "FileOutlined")),
            await EnsureMenuAsync(CreateMenu("文件目录管理", "resource-warehouse:file-directory-management", "/resource-warehouse/file-directory-management", 6, resourceWarehouse.Id, "DataWarehouse.FileDirectory", "FolderOpenOutlined")),
            await EnsureMenuAsync(CreateMenu("目录模板管理", "resource-warehouse:file-directory-template-management", "/resource-warehouse/file-directory-template-management", 7, resourceWarehouse.Id, "DataWarehouse.FileDirectoryTemplate", "ProfileOutlined")),
            await EnsureMenuAsync(CreateMenu("异步任务管理", "resource-warehouse:task-management", "/resource-warehouse/task-management", 8, resourceWarehouse.Id, "DataWarehouse.TaskManagement", "ClockCircleOutlined"))
        };

        // 地理模型（独立目录：模型定义 → 参数模板 → 执行管理，覆盖模型全生命周期）
        var geoModel = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "地理模型",
            code: "geo-model",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 70,
            Icon = "NodeIndexOutlined",
            Status = MenuStatus.Enabled
        });

        var geoModelChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("地理模型管理", "geo-model:geo-model-management", "/geo-model/geo-model-management", 1, geoModel.Id, "DataWarehouse.GeoModel", "NodeIndexOutlined")),
            await EnsureMenuAsync(CreateMenu("地理模型参数模板管理", "geo-model:parameter-template", "/geo-model/parameter-template", 2, geoModel.Id, "DataWarehouse.GeoModelTemplate", "SettingOutlined")),
            await EnsureMenuAsync(CreateMenu("地理模型执行管理", "geo-model:execution", "/geo-model/execution", 3, geoModel.Id, "DataWarehouse.GeoModelExecution", "PlayCircleOutlined"))
        };

        // 分析配置（独立目录：本体基础数据 + 专题/统计分析配置，供一张图工具使用）
        var analysisConfig = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "分析配置",
            code: "analysis-config",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 80,
            Icon = "PieChartOutlined",
            Status = MenuStatus.Enabled
        });

        var analysisConfigChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("实体类型管理", "analysis-config:entity-type", "/analysis-config/entity-type", 1, analysisConfig.Id, "DataWarehouse.EntityType", "TagOutlined")),
            await EnsureMenuAsync(CreateMenu("关系类型管理", "analysis-config:relation-type", "/analysis-config/relation-type", 2, analysisConfig.Id, "DataWarehouse.RelationType", "ShareAltOutlined")),
            await EnsureMenuAsync(CreateMenu("专题分析配置", "analysis-config:thematic-analysis", "/analysis-config/thematic-analysis", 3, analysisConfig.Id, "DataWarehouse.ThematicAnalysis", "PieChartOutlined")),
            await EnsureMenuAsync(CreateMenu("专题统计分析配置", "analysis-config:statistics-analysis", "/analysis-config/statistics-analysis", 4, analysisConfig.Id, "DataWarehouse.StatisticsAnalysis", "BarChartOutlined")),
            await EnsureMenuAsync(CreateMenu("动态表单管理", "analysis-config:form-definition", "/analysis-config/form-definition", 5, analysisConfig.Id, "DataWarehouse.FormDefinition", "FormOutlined")),
            await EnsureMenuAsync(CreateMenu("实体数据管理", "analysis-config:entity-data", "/analysis-config/entity-data", 6, analysisConfig.Id, "DataWarehouse.EntityData", "TableOutlined")),
            await EnsureMenuAsync(CreateMenu("打印模板管理", "analysis-config:print-template", "/analysis-config/print-template", 7, analysisConfig.Id, "DataWarehouse.PrintTemplate", "PrinterOutlined"))
        };

        // 资源访问控制（对应权限组 ResourceAccess = "资源访问控制"）
        var resourceManagement = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "资源访问控制",
            code: "resource-management",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 100,
            Icon = "FolderOutlined",
            Status = MenuStatus.Enabled
        });

        var resourceManagementChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("资源访问管理", "resource-management:resource-access-management", "/resource-management", 0, resourceManagement.Id, "ResourceAccess.Management", "SafetyOutlined")),
            await EnsureMenuAsync(CreateMenu("权限管理", "resource-management:permission-list", "/resource-management/permission", 1, resourceManagement.Id, "ResourceAccess.PermissionList", "KeyOutlined")),
            await EnsureMenuAsync(CreateMenu("审核历史", "resource-management:audit-history", "/resource-management/audit/history", 2, resourceManagement.Id, "ResourceAccess.AuditHistory", "HistoryOutlined")),
            await EnsureMenuAsync(CreateMenu("审核人管理", "resource-management:auditor-list", "/resource-management/auditor", 3, resourceManagement.Id, "ResourceAccess.AuditorManagement", "TeamOutlined"))
        };

        // 数据采集与治理（对应权限组 DataCollection = "数据采集与治理"）
        var dataCollection = await EnsureMenuAsync(new Menu(
            id: Guid.NewGuid(),
            name: "数据采集与治理",
            code: "data-collection",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 90,
            Icon = "DatabaseOutlined",
            Status = MenuStatus.Enabled
        });

        var dataCollectionChildren = new List<Menu>
        {
            await EnsureMenuAsync(CreateMenu("数据采集工作台", "data-collection:workbench", "/data-collection", 1, dataCollection.Id, "DataCollection.Workbench", "AppstoreOutlined")),
            await EnsureMenuAsync(CreateMenu("字典管理", "data-collection:dictionary", "/data-collection/dictionary", 2, dataCollection.Id, "DataCollection.Dictionary", "BookOutlined")),
            await EnsureMenuAsync(CreateMenu("模板与规则管理", "data-collection:template", "/data-collection/template", 3, dataCollection.Id, "DataCollection.GovernanceAdmin", "ProfileOutlined"))
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
                threeDService.Id,
                identityManagement.Id,
                messageCenter.Id,
                logsManagement.Id,
                resourceWarehouse.Id,
                resourceManagement.Id,
                dataCollection.Id,
                menuManagement.Id,
                geoModel.Id,
                analysisConfig.Id,
                home.Id,
                oneMap.Id
            };
            shouldAssign.AddRange(mapServiceChildren.Select(m => m.Id));
            shouldAssign.AddRange(threeDServiceChildren.Select(m => m.Id));
            shouldAssign.AddRange(identityChildren.Select(m => m.Id));
            shouldAssign.AddRange(messageCenterChildren.Select(m => m.Id));
            shouldAssign.AddRange(logsManagementChildren.Select(m => m.Id));
            shouldAssign.AddRange(resourceWarehouseChildren.Select(m => m.Id));
            shouldAssign.AddRange(resourceManagementChildren.Select(m => m.Id));
            shouldAssign.AddRange(dataCollectionChildren.Select(m => m.Id));
            shouldAssign.AddRange(geoModelChildren.Select(m => m.Id));
            shouldAssign.AddRange(analysisConfigChildren.Select(m => m.Id));

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
        // Permission 字段始终与种子数据保持同步，确保权限名变更后已有环境也能自动修正
        if (existing.Permission != menu.Permission) { existing.Permission = menu.Permission; changed = true; }
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
