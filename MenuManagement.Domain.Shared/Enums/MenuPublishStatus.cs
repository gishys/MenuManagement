namespace MenuManagement.Domain.Shared.Enums;

/// <summary>
/// 菜单发布状态枚举
/// 独立于 MenuStatus（启用/禁用），描述动态菜单配置的生命周期状态
/// </summary>
public enum MenuPublishStatus
{
    /// <summary>
    /// 草稿：配置中，不在运行时菜单体系生效
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 已发布：动态渲染器可读取配置渲染页面
    /// </summary>
    Published = 1,

    /// <summary>
    /// 已归档：不再提供服务，但历史记录保留
    /// </summary>
    Archived = 2
}
