using MenuManagement.Application.Contracts.DTOs;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace MenuManagement.Application.Contracts.Services;

/// <summary>
/// 菜单应用服务接口
/// </summary>
public interface IMenuAppService : ICrudAppService<MenuDto, Guid, PagedAndSortedResultRequestDto, CreateMenuDto, UpdateMenuDto>
{
    /// <summary>
    /// 获取树形菜单列表
    /// </summary>
    Task<List<MenuDto>> GetTreeAsync();

    /// <summary>
    /// 根据角色ID获取菜单
    /// </summary>
    Task<List<MenuDto>> GetMenusByRoleIdAsync(Guid roleId);

    /// <summary>
    /// 根据用户ID获取菜单
    /// </summary>
    Task<List<MenuDto>> GetMenusByUserIdAsync(Guid userId);

    /// <summary>
    /// 根据组织ID获取菜单
    /// </summary>
    Task<List<MenuDto>> GetMenusByOrganizationIdAsync(Guid organizationId);

    /// <summary>
    /// 分配菜单给角色
    /// </summary>
    Task AssignMenusToRoleAsync(Guid roleId, List<Guid> menuIds);

    /// <summary>
    /// 分配菜单给组织
    /// </summary>
    Task AssignMenusToOrganizationAsync(Guid organizationId, List<Guid> menuIds);

    /// <summary>
    /// 启用/禁用菜单
    /// </summary>
    Task SetStatusAsync(Guid id, bool enabled);
}
