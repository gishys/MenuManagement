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
        // 如果已经有菜单数据，则不重复插入
        var existingMenus = await _menuRepository.GetCountAsync();
        if (existingMenus > 0)
        {
            return;
        }

        // 地图服务管理（与前端 systemManagementMenu 首项一致，无父级）
        var mapServiceManagement = CreateMenu(
            name: "地图服务管理",
            code: "system-management",
            path: "/system-management",
            sort: 10,
            parentId: null,
            permission: "SystemManagement.MapService");
        mapServiceManagement.Icon = "AppstoreOutlined";

        // 身份管理（目录，顶级与前端一致）
        var identityManagement = new Menu(
            id: Guid.NewGuid(),
            name: "身份管理",
            code: "identity-management",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 20,
            Icon = "UserOutlined"
        };

        // 身份管理子菜单（Permission 使用与主后端/ABP 一致的权限名）
        var identityChildren = new List<Menu>
        {
            CreateMenu("用户管理", "identity-management:user", "/identity-management/user-management", 1, identityManagement.Id, "Identity.Users"),
            CreateMenu("角色管理", "identity-management:role", "/identity-management/role-management", 2, identityManagement.Id, "Identity.Roles"),
            CreateMenu("组织管理", "identity-management:organization", "/identity-management/organization-management", 3, identityManagement.Id, "Identity.OrganizationUnits"),
            CreateMenu("权限管理", "identity-management:permission", "/identity-management/permission-management", 4, identityManagement.Id, "Identity.Roles"),
            CreateMenu("关联管理", "identity-management:association", "/identity-management/association-management", 5, identityManagement.Id, "Identity.OrganizationUnits")
        };

        // 菜单管理
        var menuManagement = CreateMenu(
            name: "菜单管理",
            code: "menu-management",
            path: "/menu-management",
            sort: 30,
            parentId: null,
            permission: "SystemManagement.MenuManagement");
        menuManagement.Icon = "MenuOutlined";

        // 消息中心
        var messageCenter = new Menu(
            id: Guid.NewGuid(),
            name: "消息中心",
            code: "message-center",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 40,
            Icon = "MessageOutlined"
        };

        var messageCenterChildren = new List<Menu>
        {
            CreateMenu("消息列表", "message-center:message-list", "/message-center/message-list", 1, messageCenter.Id, "SystemManagement.MessageCenter"),
            CreateMenu("消息模板管理", "message-center:template-management", "/message-center/template-management", 2, messageCenter.Id, "SystemManagement.MessageCenter")
        };

        // 资源仓库
        var resourceWarehouse = new Menu(
            id: Guid.NewGuid(),
            name: "资源仓库",
            code: "resource-warehouse",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 50,
            Icon = "DatabaseOutlined"
        };

        var resourceWarehouseChildren = new List<Menu>
        {
            CreateMenu("二维服务", "resource-warehouse:two-dimensional-service", "/resource-warehouse/two-dimensional-service", 1, resourceWarehouse.Id, "SystemManagement.TwoDimensionalService"),
            CreateMenu("资源编目", "resource-warehouse:resource-catalog", "/resource-warehouse/resource-catalog", 2, resourceWarehouse.Id, "SystemManagement.ResourceCatalog"),
            CreateMenu("组织授权", "resource-warehouse:organization-authorization", "/resource-warehouse/organization-authorization", 3, resourceWarehouse.Id, "SystemManagement.OrganizationAuthorization"),
            CreateMenu("数据源管理", "resource-warehouse:datasource-management", "/resource-warehouse/datasource-management", 4, resourceWarehouse.Id, "SystemManagement.Datasource"),
            CreateMenu("地理模型管理", "resource-warehouse:geo-model-management", "/resource-warehouse/geo-model-management", 5, resourceWarehouse.Id, "SystemManagement.GeoModelManagement"),
            CreateMenu("地理模型参数模板管理", "resource-warehouse:geo-model-parameter-template-management", "/resource-warehouse/geo-model-parameter-template-management", 6, resourceWarehouse.Id, "SystemManagement.GeoModelParameterTemplate"),
            CreateMenu("异步任务管理", "resource-warehouse:task-management", "/resource-warehouse/task-management", 7, resourceWarehouse.Id, "SystemManagement.TaskManagement"),
            CreateMenu("文件管理", "resource-warehouse:file-management", "/resource-warehouse/file-management", 8, resourceWarehouse.Id, "SystemManagement.FileManagement"),
            CreateMenu("地理模型执行管理", "resource-warehouse:geo-model-execution-management", "/resource-warehouse/geo-model-execution-management", 9, resourceWarehouse.Id, "SystemManagement.GeoModelExecutionManagement")
        };

        // 资源管理（目录不设 Path，前端转换时 key=Id 避免与子项 path 重复；Permission 与前端 systemManagementMenu 一致）
        var resourceManagement = new Menu(
            id: Guid.NewGuid(),
            name: "资源管理",
            code: "resource-management",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 60,
            Icon = "FolderOutlined"
        };

        var resourceManagementChildren = new List<Menu>
        {
            CreateMenu("资源访问管理", "resource-management:resource-access-management", "/resource-management", 0, resourceManagement.Id, "ResourceManagement.ResourceAccessManagement"),
            CreateMenu("权限管理", "resource-management:permission-list", "/resource-management/permission", 1, resourceManagement.Id, "ResourceManagement.PermissionList"),
            CreateMenu("审核历史", "resource-management:audit-history", "/resource-management/audit/history", 2, resourceManagement.Id, "ResourceManagement.AuditHistory"),
            CreateMenu("审核人管理", "resource-management:auditor-list", "/resource-management/auditor", 3, resourceManagement.Id, "ResourceManagement.AuditorList")
        };

        // 数据采集与治理（目录不设 Path，前端 key=Id 避免重复；权限与前端 dataCollectionMenu 一致）
        var dataCollection = new Menu(
            id: Guid.NewGuid(),
            name: "数据采集",
            code: "data-collection",
            type: MenuType.Directory,
            parentId: null)
        {
            Sort = 55,
            Icon = "DatabaseOutlined"
        };

        var dataCollectionChildren = new List<Menu>
        {
            CreateMenu("数据采集工作台", "data-collection:workbench", "/data-collection", 1, dataCollection.Id, "SystemManagement.DataCollection"),
            CreateMenu("字典管理", "data-collection:dictionary", "/data-collection/dictionary", 2, dataCollection.Id, "SystemManagement.DictionaryManagement"),
            CreateMenu("模板与规则管理", "data-collection:template", "/data-collection/template", 3, dataCollection.Id, "SystemManagement.DataGovernanceAdmin")
        };

        // 默认模块简易菜单（无权限要求，Permission 为空）
        var home = CreateMenu("首页", "home", "/", 1, null, null);
        home.Icon = "HomeOutlined";

        var oneMap = CreateMenu("一张图", "page", "/page", 2, null, null);
        oneMap.Icon = "EnvironmentOutlined";

        // 按层级顺序插入（与前端扁平结构一致，无“系统管理”根节点）
        await _menuRepository.InsertManyAsync(
        [
            mapServiceManagement,
            identityManagement,
            messageCenter,
            resourceWarehouse,
            resourceManagement,
            dataCollection
        ], autoSave: true);

        await _menuRepository.InsertManyAsync(identityChildren, autoSave: true);
        await _menuRepository.InsertAsync(menuManagement, autoSave: true);
        await _menuRepository.InsertManyAsync(messageCenterChildren, autoSave: true);
        await _menuRepository.InsertManyAsync(resourceWarehouseChildren, autoSave: true);
        await _menuRepository.InsertManyAsync(resourceManagementChildren, autoSave: true);
        await _menuRepository.InsertManyAsync(dataCollectionChildren, autoSave: true);
        await _menuRepository.InsertManyAsync([home, oneMap], autoSave: true);

        // 为 admin 角色赋予所有菜单权限
        var adminRole = await _roleRepository.FindByNormalizedNameAsync(AdminRoleNormalizedName);
        if (adminRole != null)
        {
            var allMenuIds = new List<Guid>
            {
                mapServiceManagement.Id,
                identityManagement.Id,
                messageCenter.Id,
                resourceWarehouse.Id,
                resourceManagement.Id,
                dataCollection.Id,
                menuManagement.Id,
                home.Id,
                oneMap.Id
            };
            allMenuIds.AddRange(identityChildren.Select(m => m.Id));
            allMenuIds.AddRange(messageCenterChildren.Select(m => m.Id));
            allMenuIds.AddRange(resourceWarehouseChildren.Select(m => m.Id));
            allMenuIds.AddRange(resourceManagementChildren.Select(m => m.Id));
            allMenuIds.AddRange(dataCollectionChildren.Select(m => m.Id));
            await _menuRepository.ReplaceMenusForRoleAsync(adminRole.Id, allMenuIds);
        }
    }

    /// <param name="permission">ABP 权限名（与主后端 MenuPermissionNames 一致）；为 null 时表示无权限要求（如首页、一张图）</param>
    private static Menu CreateMenu(
        string name,
        string code,
        string path,
        int sort,
        Guid? parentId = null,
        string? permission = null)
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
            Permission = permission
        };
    }
}

