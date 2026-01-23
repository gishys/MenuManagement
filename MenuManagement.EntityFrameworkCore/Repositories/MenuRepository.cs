using MenuManagement.Domain.Entities;
using MenuManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace MenuManagement.EntityFrameworkCore.Repositories;

/// <summary>
/// 菜单仓储实现
/// </summary>
public class MenuRepository(
    IDbContextProvider<MenuManagementDbContext> dbContextProvider,
    IIdentityUserRepository userRepository,
    IIdentityRoleRepository roleRepository) : EfCoreRepository<MenuManagementDbContext, Menu, Guid>(dbContextProvider), IMenuRepository
{
    private readonly IIdentityUserRepository _userRepository = userRepository;
    private readonly IIdentityRoleRepository _roleRepository = roleRepository;

    public async Task<Menu?> GetByCodeAsync(string code, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = includeDetails ? query.Include(x => x.Children).Include(x => x.Parent) : query;
        return await query.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
    }

    public async Task<List<Menu>> GetEnabledMenusAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = includeDetails ? query.Include(x => x.Children).Include(x => x.Parent) : query;
        return await query
            .Where(x => x.Status == Domain.Shared.Enums.MenuStatus.Enabled)
            .OrderBy(x => x.Sort)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Menu>> GetChildrenAsync(Guid? parentId, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = includeDetails ? query.Include(x => x.Children).Include(x => x.Parent) : query;
        return await query
            .Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Sort)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Menu>> GetMenusByRoleIdAsync(Guid roleId, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = includeDetails ? query.Include(x => x.Children).Include(x => x.Parent).Include(x => x.MenuRoles) : query;
        return await query
            .Where(x => x.MenuRoles.Any(mr => mr.RoleId == roleId) && x.Status == Domain.Shared.Enums.MenuStatus.Enabled)
            .OrderBy(x => x.Sort)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Menu>> GetMenusByUserIdAsync(Guid userId, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(userId, cancellationToken: cancellationToken);
        var userRoles = user.Roles.Select(r => r.RoleId).ToList();

        var query = await GetQueryableAsync();
        query = includeDetails ? query.Include(x => x.Children).Include(x => x.Parent).Include(x => x.MenuRoles) : query;
        return await query
            .Where(x => x.MenuRoles.Any(mr => userRoles.Contains(mr.RoleId)) && x.Status == Domain.Shared.Enums.MenuStatus.Enabled)
            .OrderBy(x => x.Sort)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Menu>> GetMenusByOrganizationIdAsync(Guid organizationId, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = includeDetails 
            ? query.Include(x => x.Children).Include(x => x.Parent).Include(x => x.MenuOrganizations) 
            : query.Include(x => x.MenuOrganizations);
        return await query
            .Where(x => x.MenuOrganizations.Any(mo => mo.OrganizationUnitId == organizationId) && x.Status == Domain.Shared.Enums.MenuStatus.Enabled)
            .OrderBy(x => x.Sort)
            .ToListAsync(cancellationToken);
    }
}
