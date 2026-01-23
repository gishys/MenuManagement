using MenuManagement.Domain.Shared.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace MenuManagement.Domain.Entities;

/// <summary>
/// 菜单实体
/// </summary>
public class Menu : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单类型
    /// </summary>
    public MenuType Type { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    public string? Component { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public MenuStatus Status { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string? Permission { get; set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// 是否缓存
    /// </summary>
    public bool IsCache { get; set; }

    /// <summary>
    /// 是否外链
    /// </summary>
    public bool IsExternal { get; set; }

    /// <summary>
    /// 外链地址
    /// </summary>
    public string? ExternalUrl { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 父菜单导航属性
    /// </summary>
    public virtual Menu? Parent { get; set; }

    /// <summary>
    /// 子菜单集合
    /// </summary>
    public virtual ICollection<Menu> Children { get; set; } = [];

    /// <summary>
    /// 菜单角色关联
    /// </summary>
    public virtual ICollection<MenuRole> MenuRoles { get; set; } = [];

    /// <summary>
    /// 菜单组织关联
    /// </summary>
    public virtual ICollection<MenuOrganization> MenuOrganizations { get; set; } = [];

    protected Menu()
    {
    }

    public Menu(
        Guid id,
        string name,
        string code,
        MenuType type,
        Guid? parentId = null)
        : base(id)
    {
        Name = name;
        Code = code;
        Type = type;
        ParentId = parentId;
        Status = MenuStatus.Enabled;
    }
}
