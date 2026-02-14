using MenuManagement.Domain.Entities;
using MenuManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace MenuManagement.EntityFrameworkCore.Repositories;

/// <summary>
/// 菜单仓储实现
/// </summary>
public class MenuRepository(IDbContextProvider<MenuManagementDbContext> dbContextProvider)
    : EfCoreRepository<MenuManagementDbContext, Menu, Guid>(dbContextProvider), IMenuRepository
{
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
        return await GetMenusByRoleIdsAsync([roleId], includeDetails, cancellationToken);
    }

    public async Task<List<Menu>> GetMenusByRoleIdsAsync(IEnumerable<Guid> roleIds, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var roleIdList = roleIds.ToList();
        if (roleIdList.Count == 0)
        {
            return [];
        }
        var query = await GetQueryableAsync();
        query = includeDetails ? query.Include(x => x.Children).Include(x => x.Parent).Include(x => x.MenuRoles) : query;
        return await query
            .Where(x => x.MenuRoles.Any(mr => roleIdList.Contains(mr.RoleId)) && x.Status == Domain.Shared.Enums.MenuStatus.Enabled)
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

    public async Task<List<Menu>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0)
        {
            return [];
        }
        var query = await GetQueryableAsync();
        return await query.Where(x => idList.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task ReplaceMenusForRoleAsync(Guid roleId, List<Guid> menuIds, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var existingMenuRoles = await dbContext.MenuRoles
            .Where(mr => mr.RoleId == roleId)
            .ToListAsync(cancellationToken);
        dbContext.MenuRoles.RemoveRange(existingMenuRoles);
        foreach (var menuId in menuIds)
        {
            await dbContext.MenuRoles.AddAsync(new MenuRole(menuId, roleId), cancellationToken);
        }
    }

    public async Task ReplaceMenusForOrganizationAsync(Guid organizationId, List<Guid> menuIds, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        var existingMenuOrganizations = await dbContext.MenuOrganizations
            .Where(mo => mo.OrganizationUnitId == organizationId)
            .ToListAsync(cancellationToken);
        dbContext.MenuOrganizations.RemoveRange(existingMenuOrganizations);
        foreach (var menuId in menuIds)
        {
            await dbContext.MenuOrganizations.AddAsync(new MenuOrganization(menuId, organizationId), cancellationToken);
        }
    }
}
