using MenuManagement.Application.Contracts.DTOs;
using MenuManagement.Application.Contracts.Services;
using MenuManagement.Domain.Entities;
using MenuManagement.Domain.Repositories;
using MenuManagement.Domain.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace MenuManagement.Application.Services;

/// <summary>
/// 菜单应用服务
/// </summary>
[Authorize]
public class MenuAppService(
    IMenuRepository repository,
    IIdentityRoleRepository roleRepository,
    IIdentityUserRepository userRepository,
    IDbContextProvider<MenuManagement.EntityFrameworkCore.MenuManagementDbContext> dbContextProvider)
    : CrudAppService<Menu, MenuDto, Guid, PagedAndSortedResultRequestDto, CreateMenuDto, UpdateMenuDto>(repository), IMenuAppService
{
    private readonly IMenuRepository _menuRepository = repository;
    private readonly IIdentityRoleRepository _roleRepository = roleRepository;
    private readonly IIdentityUserRepository _userRepository = userRepository;
    private readonly IDbContextProvider<MenuManagement.EntityFrameworkCore.MenuManagementDbContext> _dbContextProvider = dbContextProvider;

    private async Task<MenuManagement.EntityFrameworkCore.MenuManagementDbContext> GetDbContextAsync()
    {
        return await _dbContextProvider.GetDbContextAsync();
    }

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
    /// 根据角色ID获取菜单
    /// </summary>
    public async Task<List<MenuDto>> GetMenusByRoleIdAsync(Guid roleId)
    {
        var menus = await _menuRepository.GetMenusByRoleIdAsync(roleId);
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
    /// 根据用户ID获取菜单
    /// </summary>
    public async Task<List<MenuDto>> GetMenusByUserIdAsync(Guid userId)
    {
        var menus = await _menuRepository.GetMenusByUserIdAsync(userId);
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
    /// 根据组织ID获取菜单
    /// </summary>
    public async Task<List<MenuDto>> GetMenusByOrganizationIdAsync(Guid organizationId)
    {
        var menus = await _menuRepository.GetMenusByOrganizationIdAsync(organizationId);
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
    /// 分配菜单给角色
    /// </summary>
    public async Task AssignMenusToRoleAsync(Guid roleId, List<Guid> menuIds)
    {
        var role = await _roleRepository.GetAsync(roleId);

        // 清除现有关联
        var dbContext = await GetDbContextAsync();
        var existingMenuRoles = await dbContext.MenuRoles
            .Where(mr => mr.RoleId == roleId)
            .ToListAsync();
        dbContext.MenuRoles.RemoveRange(existingMenuRoles);

        // 添加新关联
        foreach (var menuId in menuIds)
        {
            var menuRole = new MenuRole(menuId, roleId);
            await dbContext.MenuRoles.AddAsync(menuRole);
        }

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// 分配菜单给组织
    /// </summary>
    public async Task AssignMenusToOrganizationAsync(Guid organizationId, List<Guid> menuIds)
    {
        // 清除现有关联
        var dbContext = await GetDbContextAsync();
        var existingMenuOrganizations = await dbContext.MenuOrganizations
            .Where(mo => mo.OrganizationUnitId == organizationId)
            .ToListAsync();
        dbContext.MenuOrganizations.RemoveRange(existingMenuOrganizations);

        // 添加新关联
        foreach (var menuId in menuIds)
        {
            var menuOrganization = new MenuOrganization(menuId, organizationId);
            await dbContext.MenuOrganizations.AddAsync(menuOrganization);
        }

        await dbContext.SaveChangesAsync();
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
            Remark = createInput.Remark
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

        await Task.CompletedTask;
    }
}
