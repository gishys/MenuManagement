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
    /// 根据用户ID获取菜单
    /// </summary>
    Task<List<Menu>> GetMenusByUserIdAsync(Guid userId, bool includeDetails = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据组织ID获取菜单
    /// </summary>
    Task<List<Menu>> GetMenusByOrganizationIdAsync(Guid organizationId, bool includeDetails = false, CancellationToken cancellationToken = default);
}
