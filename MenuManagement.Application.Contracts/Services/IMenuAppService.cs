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

    /// <summary>
    /// 获取当前用户可访问的菜单权限标识列表（用于方案 B 前端按叶子权限过滤）
    /// </summary>
    Task<List<string>> GetMyMenuPermissionsAsync();

    /// <summary>
    /// 按主体获取可访问的菜单权限标识列表。providerName: U=用户,R=角色,O=组织；providerKey: 对应 ID
    /// </summary>
    Task<List<string>> GetMenuPermissionsAsync(string providerName, string providerKey);

    /// <summary>
    /// 发布菜单动态配置（Draft → Published）
    /// </summary>
    Task PublishAsync(Guid id);

    /// <summary>
    /// 下线菜单动态配置（Published → Draft）
    /// </summary>
    Task UnpublishAsync(Guid id);

    /// <summary>
    /// 归档菜单动态配置（→ Archived）
    /// </summary>
    Task ArchiveAsync(Guid id);

    /// <summary>
    /// 根据菜单编码查询菜单（运行时渲染器使用）
    /// </summary>
    Task<MenuDto> GetByCodeAsync(string code);

    /// <summary>
    /// 获取已发布状态的菜单树（供运行时侧边栏/导航使用）
    /// </summary>
    Task<List<MenuDto>> GetPublishedTreeAsync();
}
