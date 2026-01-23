using Volo.Abp.Domain.Entities;

namespace MenuManagement.Domain.Entities;

/// <summary>
/// 菜单组织关联实体
/// </summary>
public class MenuOrganization : Entity
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid MenuId { get; set; }

    /// <summary>
    /// 组织ID（ABP Identity的OrganizationUnit Id）
    /// </summary>
    public Guid OrganizationUnitId { get; set; }

    /// <summary>
    /// 菜单导航属性
    /// </summary>
    public virtual Menu? Menu { get; set; }

    protected MenuOrganization()
    {
    }

    public MenuOrganization(Guid menuId, Guid organizationUnitId)
    {
        MenuId = menuId;
        OrganizationUnitId = organizationUnitId;
    }

    public override object[] GetKeys()
    {
        return [MenuId, OrganizationUnitId];
    }
}
