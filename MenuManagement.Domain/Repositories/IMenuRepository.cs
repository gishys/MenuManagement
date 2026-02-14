using MenuManagement.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace MenuManagement.Domain.Repositories;

/// <summary>
/// 菜单仓储接口
/// </summary>
public interface IMenuRepository : IRepository<Menu, Guid>
{
    /// <summary>
    /// 根据编码获取菜单
    /// </summary>
    Task<Menu?> GetByCodeAsync(string code, bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有启用的菜单
    /// </summary>
    Task<List<Menu>> GetEnabledMenusAsync(bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据父ID获取子菜单
    /// </summary>
    Task<List<Menu>> GetChildrenAsync(Guid? parentId, bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据角色ID获取菜单
    /// </summary>
    Task<List<Menu>> GetMenusByRoleIdAsync(Guid roleId, bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据角色ID列表获取菜单（多角色并集，基于现有 MenuRole 查询）
    /// </summary>
    Task<List<Menu>> GetMenusByRoleIdsAsync(IEnumerable<Guid> roleIds, bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据组织ID获取菜单
    /// </summary>
    Task<List<Menu>> GetMenusByOrganizationIdAsync(Guid organizationId, bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 列表批量获取菜单
    /// </summary>
    Task<List<Menu>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按角色替换菜单分配（先清除该角色现有关联，再添加新关联）
    /// </summary>
    Task ReplaceMenusForRoleAsync(Guid roleId, List<Guid> menuIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按组织替换菜单分配（先清除该组织现有关联，再添加新关联）
    /// </summary>
    Task ReplaceMenusForOrganizationAsync(Guid organizationId, List<Guid> menuIds, CancellationToken cancellationToken = default);
}
