using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MenuManagement.Domain.Entities;
using MenuManagement.Domain.Repositories;
using MenuManagement.Domain.Shared.Enums;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;

namespace MenuManagement.Domain;

/// <summary>
/// 菜单种子数据（基于前端菜单配置）
/// </summary>
public class MenuSeedDataContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IMenuRepository _menuRepository;

    public MenuSeedDataContributor(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 如果已经有菜单数据，则不重复插入
        var existingMenus = await _menuRepository.GetCountAsync();
        if (existingMenus > 0)
        {
            return;
        }

        // 根：系统管理模块
        var systemManagement = new Menu(
            id: Guid.NewGuid(),
            name: "系统管理",
            code: "system-management",
            type: MenuType.Directory);
        systemManagement.Path = "/system-management";
        systemManagement.Sort = 10;
        systemManagement.Icon = "AppstoreOutlined";

        // 身份管理（目录）
        var identityManagement = new Menu(
            id: Guid.NewGuid(),
            name: "身份管理",
            code: "identity-management",
            type: MenuType.Directory,
            parentId: systemManagement.Id);
        identityManagement.Sort = 20;
        identityManagement.Icon = "UserOutlined";

        // 身份管理子菜单
        var identityChildren = new List<Menu>
        {
            CreateMenu("用户管理", "identity-management:user", "/identity-management/user-management", 1, identityManagement.Id),
            CreateMenu("角色管理", "identity-management:role", "/identity-management/role-management", 2, identityManagement.Id),
            CreateMenu("组织管理", "identity-management:organization", "/identity-management/organization-management", 3, identityManagement.Id),
            CreateMenu("权限管理", "identity-management:permission", "/identity-management/permission-management", 4, identityManagement.Id),
            CreateMenu("关联管理", "identity-management:association", "/identity-management/association-management", 5, identityManagement.Id)
        };

        // 菜单管理
        var menuManagement = CreateMenu(
            name: "菜单管理",
            code: "menu-management",
            path: "/menu-management",
            sort: 30,
            parentId: systemManagement.Id);
        menuManagement.Icon = "MenuOutlined";

        // 消息中心
        var messageCenter = new Menu(
            id: Guid.NewGuid(),
            name: "消息中心",
            code: "message-center",
            type: MenuType.Directory,
            parentId: systemManagement.Id);
        messageCenter.Sort = 40;
        messageCenter.Icon = "MessageOutlined";

        var messageCenterChildren = new List<Menu>
        {
            CreateMenu("消息列表", "message-center:message-list", "/message-center/message-list", 1, messageCenter.Id),
            CreateMenu("消息模板管理", "message-center:template-management", "/message-center/template-management", 2, messageCenter.Id)
        };

        // 资源仓库
        var resourceWarehouse = new Menu(
            id: Guid.NewGuid(),
            name: "资源仓库",
            code: "resource-warehouse",
            type: MenuType.Directory,
            parentId: systemManagement.Id);
        resourceWarehouse.Sort = 50;
        resourceWarehouse.Icon = "DatabaseOutlined";

        var resourceWarehouseChildren = new List<Menu>
        {
            CreateMenu("二维服务", "resource-warehouse:two-dimensional-service", "/resource-warehouse/two-dimensional-service", 1, resourceWarehouse.Id),
            CreateMenu("资源编目", "resource-warehouse:resource-catalog", "/resource-warehouse/resource-catalog", 2, resourceWarehouse.Id),
            CreateMenu("组织授权", "resource-warehouse:organization-authorization", "/resource-warehouse/organization-authorization", 3, resourceWarehouse.Id),
            CreateMenu("数据源管理", "resource-warehouse:datasource-management", "/resource-warehouse/datasource-management", 4, resourceWarehouse.Id),
            CreateMenu("地理模型管理", "resource-warehouse:geo-model-management", "/resource-warehouse/geo-model-management", 5, resourceWarehouse.Id),
            CreateMenu("地理模型参数模板管理", "resource-warehouse:geo-model-parameter-template-management", "/resource-warehouse/geo-model-parameter-template-management", 6, resourceWarehouse.Id),
            CreateMenu("异步任务管理", "resource-warehouse:task-management", "/resource-warehouse/task-management", 7, resourceWarehouse.Id),
            CreateMenu("文件管理", "resource-warehouse:file-management", "/resource-warehouse/file-management", 8, resourceWarehouse.Id),
            CreateMenu("地理模型执行管理", "resource-warehouse:geo-model-execution-management", "/resource-warehouse/geo-model-execution-management", 9, resourceWarehouse.Id)
        };

        // 默认模块简易菜单（对应前端 menuConfig）
        var home = CreateMenu("首页", "home", "/", 1);
        home.Icon = "HomeOutlined";

        var formSample = CreateMenu("表单示例", "form-sample", "/form", 2);
        formSample.Icon = "FormOutlined";

        var tableSample = CreateMenu("表格示例", "table-sample", "/table", 3);
        tableSample.Icon = "TableOutlined";

        // 按层级顺序插入
        await _menuRepository.InsertManyAsync(new[]
        {
            systemManagement,
            identityManagement,
            messageCenter,
            resourceWarehouse
        }, autoSave: true);

        await _menuRepository.InsertManyAsync(identityChildren, autoSave: true);
        await _menuRepository.InsertAsync(menuManagement, autoSave: true);
        await _menuRepository.InsertManyAsync(messageCenterChildren, autoSave: true);
        await _menuRepository.InsertManyAsync(resourceWarehouseChildren, autoSave: true);
        await _menuRepository.InsertManyAsync(new[] { home, formSample, tableSample }, autoSave: true);
    }

    private static Menu CreateMenu(
        string name,
        string code,
        string path,
        int sort,
        Guid? parentId = null)
    {
        var menu = new Menu(
            id: Guid.NewGuid(),
            name: name,
            code: code,
            type: MenuType.Menu,
            parentId: parentId);

        menu.Path = path;
        menu.Sort = sort;
        menu.Status = MenuStatus.Enabled;

        return menu;
    }
}

