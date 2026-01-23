using Volo.Abp.Domain.Entities;

namespace MenuManagement.Domain.Entities;

/// <summary>
/// 菜单角色关联实体
/// </summary>
public class MenuRole : Entity
{
    /// <summary>
    /// 菜单ID
    /// </summary>
    public Guid MenuId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 菜单导航属性
    /// </summary>
    public virtual Menu? Menu { get; set; }

    protected MenuRole()
    {
    }

    public MenuRole(Guid menuId, Guid roleId)
    {
        MenuId = menuId;
        RoleId = roleId;
    }

    public override object[] GetKeys()
    {
        return [MenuId, RoleId];
    }
}
