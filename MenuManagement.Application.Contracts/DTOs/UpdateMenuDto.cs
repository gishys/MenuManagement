using MenuManagement.Domain.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace MenuManagement.Application.Contracts.DTOs;

/// <summary>
/// 更新菜单DTO
/// </summary>
public class UpdateMenuDto
{
    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 菜单编码
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单类型
    /// </summary>
    [Required]
    public MenuType Type { get; set; }

    /// <summary>
    /// 路由路径
    /// </summary>
    [StringLength(200)]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [StringLength(200)]
    public string? Component { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [StringLength(50)]
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
    [StringLength(100)]
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
    [StringLength(500)]
    public string? ExternalUrl { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500)]
    public string? Remark { get; set; }
}
