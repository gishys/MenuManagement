using MenuManagement.Application.Contracts.DTOs;
using MenuManagement.Application.Contracts.Services;
using MenuManagement.Domain.Entities;
using MenuManagement.Domain.Repositories;
using MenuManagement.Domain.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace MenuManagement.Application.Services;

/// <summary>
/// 菜单应用服务
/// </summary>
[Authorize]
public class MenuAppService(
    IMenuRepository repository,
    IIdentityRoleRepository roleRepository,
    IdentityUserManager userManager,
    ICurrentUser currentUser)
    : CrudAppService<Menu, MenuDto, Guid, PagedAndSortedResultRequestDto, CreateMenuDto, UpdateMenuDto>(repository), IMenuAppService
{
    private readonly IMenuRepository _menuRepository = repository;
    private readonly IIdentityRoleRepository _roleRepository = roleRepository;
    private readonly IdentityUserManager _userManager = userManager;
    private readonly ICurrentUser _currentUser = currentUser;

    /// <summary>
    /// 获取树形菜单列表
    /// </summary>
    public async Task<List<MenuDto>> GetTreeAsync()
    {
        var menus = await _menuRepository.GetListAsync();
        var menuDtos = ObjectMapper.Map<List<Menu>, List<MenuDto>>(menus);

        // 构建树形结构
        var rootMenus = menuDtos.Where(m => m.ParentId == null).OrderBy(m => m.Sort).ToList();
        foreach (var rootMenu in rootMenus)
        {
            BuildMenuTree(rootMenu, menuDtos);
        }

        return rootMenus;
    }

    /// <summary>
    /// 根据角色ID获取菜单（含祖先链，仅返回已分配节点及其到根的路径）
    /// </summary>
    public async Task<List<MenuDto>> GetMenusByRoleIdAsync(Guid roleId)
    {
        var menus = await _menuRepository.GetMenusByRoleIdAsync(roleId);
        var withAncestors = await EnsureAncestorsAsync(menus);
        var menuDtos = ObjectMapper.Map<List<Menu>, List<MenuDto>>(withAncestors);

        var rootMenus = menuDtos.Where(m => m.ParentId == null).OrderBy(m => m.Sort).ToList();
        foreach (var rootMenu in rootMenus)
        {
            BuildMenuTree(rootMenu, menuDtos);
        }

        return rootMenus;
    }

    /// <summary>
    /// 根据用户ID获取菜单（用户→角色+组织→菜单，含祖先链，仅返回已分配节点及其到根的路径）
    /// </summary>
    public async Task<List<MenuDto>> GetMenusByUserIdAsync(Guid userId)
    {
        var (roleIds, organizationIds) = await GetUserRoleAndOrganizationIdsAsync(userId);
        var menusByRole = await _menuRepository.GetMenusByRoleIdsAsync(roleIds);
        var allMenus = menusByRole.ToDictionary(m => m.Id, m => m);
        foreach (var orgId in organizationIds)
        {
            var menusByOrg = await _menuRepository.GetMenusByOrganizationIdAsync(orgId);
            foreach (var m in menusByOrg)
            {
                if (!allMenus.ContainsKey(m.Id))
                {
                    allMenus[m.Id] = m;
                }
            }
        }
        var merged = allMenus.Values.ToList();
        var withAncestors = await EnsureAncestorsAsync(merged);
        var menuDtos = ObjectMapper.Map<List<Menu>, List<MenuDto>>(withAncestors);

        var rootMenus = menuDtos.Where(m => m.ParentId == null).OrderBy(m => m.Sort).ToList();
        foreach (var rootMenu in rootMenus)
        {
            BuildMenuTree(rootMenu, menuDtos);
        }

        return rootMenus;
    }

    /// <summary>
    /// 解析用户对应的角色ID列表与组织ID列表（用户→角色+组织）
    /// </summary>
    private async Task<(List<Guid> roleIds, List<Guid> organizationIds)> GetUserRoleAndOrganizationIdsAsync(Guid userId)
    {
        var user = await _userManager.GetByIdAsync(userId);
        if (user == null)
        {
            return ([], []);
        }
        var roleNames = await _userManager.GetRolesAsync(user);
        var roleIds = await ResolveRoleNamesToIdsAsync(roleNames);
        var organizationIds = await GetUserOrganizationIdsAsync(user);
        return (roleIds, organizationIds);
    }

    private async Task<List<Guid>> ResolveRoleNamesToIdsAsync(IList<string> roleNames)
    {
        if (roleNames == null || roleNames.Count == 0)
        {
            return [];
        }
        var roleNameSet = roleNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var allRoles = await _roleRepository.GetListAsync();
        return [..allRoles.Where(r => roleNameSet.Contains(r.Name ?? "")).Select(r => r.Id)];
    }

    private async Task<List<Guid>> GetUserOrganizationIdsAsync(Volo.Abp.Identity.IdentityUser user)
    {
        try
        {
            var orgUnits = await _userManager.GetOrganizationUnitsAsync(user);
            return orgUnits?.Select(ou => ou.Id).ToList() ?? [];
        }
        catch
        {
            return [];
        }
    }

    /// <summary>
    /// 根据组织ID获取菜单（含祖先链，仅返回已分配节点及其到根的路径）
    /// </summary>
    public async Task<List<MenuDto>> GetMenusByOrganizationIdAsync(Guid organizationId)
    {
        var menus = await _menuRepository.GetMenusByOrganizationIdAsync(organizationId);
        var withAncestors = await EnsureAncestorsAsync(menus);
        var menuDtos = ObjectMapper.Map<List<Menu>, List<MenuDto>>(withAncestors);

        var rootMenus = menuDtos.Where(m => m.ParentId == null).OrderBy(m => m.Sort).ToList();
        foreach (var rootMenu in rootMenus)
        {
            BuildMenuTree(rootMenu, menuDtos);
        }

        return rootMenus;
    }

    /// <summary>
    /// 补全祖先链：在已分配菜单列表上加入所有祖先节点，便于构建完整树
    /// </summary>
    private async Task<List<Menu>> EnsureAncestorsAsync(List<Menu> assignedMenus)
    {
        if (assignedMenus.Count == 0)
        {
            return [];
        }
        var idSet = assignedMenus.Select(m => m.Id).ToHashSet();
        var toLoad = assignedMenus.Select(m => m.ParentId).Where(id => id.HasValue).Select(id => id!.Value).Where(id => !idSet.Contains(id)).ToHashSet();
        while (toLoad.Count > 0)
        {
            var ancestors = await _menuRepository.GetByIdsAsync(toLoad);
            foreach (var a in ancestors)
            {
                idSet.Add(a.Id);
            }
            toLoad = [..ancestors.Select(a => a.ParentId).Where(id => id.HasValue).Select(id => id!.Value).Where(id => !idSet.Contains(id))];
        }
        return await _menuRepository.GetByIdsAsync(idSet);
    }

    /// <summary>
    /// 分配菜单给角色
    /// </summary>
    public async Task AssignMenusToRoleAsync(Guid roleId, List<Guid> menuIds)
    {
        await _roleRepository.GetAsync(roleId);
        await _menuRepository.ReplaceMenusForRoleAsync(roleId, menuIds ?? []);
    }

    /// <summary>
    /// 分配菜单给组织
    /// </summary>
    public async Task AssignMenusToOrganizationAsync(Guid organizationId, List<Guid> menuIds)
    {
        await _menuRepository.ReplaceMenusForOrganizationAsync(organizationId, menuIds ?? []);
    }

    /// <summary>
    /// 启用/禁用菜单
    /// </summary>
    public async Task SetStatusAsync(Guid id, bool enabled)
    {
        var menu = await _menuRepository.GetAsync(id);
        menu.Status = enabled ? MenuStatus.Enabled : MenuStatus.Disabled;
        await _menuRepository.UpdateAsync(menu);
    }

    /// <summary>
    /// 获取当前用户可访问的菜单权限标识列表（用于方案 B 前端按叶子权限过滤）
    /// </summary>
    public async Task<List<string>> GetMyMenuPermissionsAsync()
    {
        var userId = _currentUser.Id;
        if (userId == null)
        {
            return [];
        }
        return await GetMenuPermissionsAsync("U", userId.Value.ToString());
    }

    /// <summary>
    /// 按主体获取可访问的菜单权限标识列表。providerName: U=用户,R=角色,O=组织；providerKey: 对应 ID
    /// </summary>
    public async Task<List<string>> GetMenuPermissionsAsync(string providerName, string providerKey)
    {
        if (string.IsNullOrWhiteSpace(providerKey) || !Guid.TryParse(providerKey, out var key))
        {
            return [];
        }
        List<Menu> menus;
        switch (providerName?.ToUpperInvariant())
        {
            case "U":
                var (roleIds, organizationIds) = await GetUserRoleAndOrganizationIdsAsync(key);
                var byRole = await _menuRepository.GetMenusByRoleIdsAsync(roleIds);
                var byId = byRole.ToDictionary(m => m.Id, m => m);
                foreach (var orgId in organizationIds)
                {
                    var byOrg = await _menuRepository.GetMenusByOrganizationIdAsync(orgId);
                    foreach (var m in byOrg)
                    {
                        if (!byId.ContainsKey(m.Id))
                        {
                            byId[m.Id] = m;
                        }
                    }
                }
                menus = [..byId.Values];
                break;
            case "R":
                menus = await _menuRepository.GetMenusByRoleIdAsync(key);
                break;
            case "O":
                menus = await _menuRepository.GetMenusByOrganizationIdAsync(key);
                break;
            default:
                return [];
        }
        return [..menus
            .Where(m => !string.IsNullOrWhiteSpace(m.Permission))
            .Select(m => m.Permission!)
            .Distinct()];
    }

    /// <summary>
    /// 构建菜单树
    /// </summary>
    private static void BuildMenuTree(MenuDto parentMenu, List<MenuDto> allMenus)
    {
        var children = allMenus.Where(m => m.ParentId == parentMenu.Id).OrderBy(m => m.Sort).ToList();
        parentMenu.Children = children;
        foreach (var child in children)
        {
            BuildMenuTree(child, allMenus);
        }
    }

    protected override Task<Menu> MapToEntityAsync(CreateMenuDto createInput)
    {
        var menu = new Menu(
            GuidGenerator.Create(),
            createInput.Name,
            createInput.Code,
            createInput.Type,
            createInput.ParentId)
        {
            Path = createInput.Path,
            Component = createInput.Component,
            Icon = createInput.Icon,
            Sort = createInput.Sort,
            Status = createInput.Status,
            Permission = createInput.Permission,
            IsHidden = createInput.IsHidden,
            IsCache = createInput.IsCache,
            IsExternal = createInput.IsExternal,
            ExternalUrl = createInput.ExternalUrl,
            Remark = createInput.Remark,
            FeatureType = createInput.FeatureType,
            DynamicConfig = createInput.DynamicConfig
        };

        return Task.FromResult(menu);
    }

    protected override async Task MapToEntityAsync(UpdateMenuDto updateInput, Menu entity)
    {
        entity.Name = updateInput.Name;
        entity.Code = updateInput.Code;
        entity.ParentId = updateInput.ParentId;
        entity.Type = updateInput.Type;
        entity.Path = updateInput.Path;
        entity.Component = updateInput.Component;
        entity.Icon = updateInput.Icon;
        entity.Sort = updateInput.Sort;
        entity.Status = updateInput.Status;
        entity.Permission = updateInput.Permission;
        entity.IsHidden = updateInput.IsHidden;
        entity.IsCache = updateInput.IsCache;
        entity.IsExternal = updateInput.IsExternal;
        entity.ExternalUrl = updateInput.ExternalUrl;
        entity.Remark = updateInput.Remark;
        entity.FeatureType = updateInput.FeatureType;
        entity.DynamicConfig = updateInput.DynamicConfig;

        await Task.CompletedTask;
    }

    /// <summary>
    /// 发布菜单动态配置（Draft → Published）
    /// </summary>
    public async Task PublishAsync(Guid id)
    {
        var menu = await _menuRepository.GetAsync(id);
        menu.PublishStatus = MenuPublishStatus.Published;
        await _menuRepository.UpdateAsync(menu);
    }

    /// <summary>
    /// 下线菜单动态配置（Published → Draft）
    /// </summary>
    public async Task UnpublishAsync(Guid id)
    {
        var menu = await _menuRepository.GetAsync(id);
        menu.PublishStatus = MenuPublishStatus.Draft;
        await _menuRepository.UpdateAsync(menu);
    }

    /// <summary>
    /// 归档菜单动态配置（→ Archived）
    /// </summary>
    public async Task ArchiveAsync(Guid id)
    {
        var menu = await _menuRepository.GetAsync(id);
        menu.PublishStatus = MenuPublishStatus.Archived;
        await _menuRepository.UpdateAsync(menu);
    }

    /// <summary>
    /// 根据菜单编码查询菜单（运行时渲染器使用）
    /// </summary>
    public async Task<MenuDto> GetByCodeAsync(string code)
    {
        var menus = await _menuRepository.GetListAsync(m => m.Code == code);
        var menu = menus.FirstOrDefault()
            ?? throw new Volo.Abp.UserFriendlyException($"菜单编码 '{code}' 不存在");
        return ObjectMapper.Map<Menu, MenuDto>(menu);
    }

    /// <summary>
    /// 获取已发布状态的菜单树（供运行时侧边栏/导航使用）
    /// </summary>
    public async Task<List<MenuDto>> GetPublishedTreeAsync()
    {
        var menus = await _menuRepository.GetListAsync(
            m => m.PublishStatus == MenuPublishStatus.Published && m.Status == MenuStatus.Enabled);
        var menuDtos = ObjectMapper.Map<List<Menu>, List<MenuDto>>(menus);

        var rootMenus = menuDtos.Where(m => m.ParentId == null).OrderBy(m => m.Sort).ToList();
        foreach (var rootMenu in rootMenus)
        {
            BuildMenuTree(rootMenu, menuDtos);
        }

        return rootMenus;
    }
}
