namespace MenuManagement.Domain.Shared.Enums;

/// <summary>
/// 菜单功能类型枚举
/// 描述动态菜单在运行时渲染的页面能力类型
/// </summary>
public enum MenuFeatureType
{
    /// <summary>
    /// 普通菜单（静态组件路径，无动态配置）
    /// </summary>
    None = 0,

    /// <summary>
    /// 查询分析页：条件表单 + 结果列表 + 导出
    /// </summary>
    DataQuery = 1,

    /// <summary>
    /// 数据管理页：查询筛选 + 列表 + 新建/编辑/查看/删除
    /// </summary>
    DataManagement = 2,

    /// <summary>
    /// 表单填写页：单一表单提交（如申请、上报）
    /// </summary>
    FormFill = 3,

    /// <summary>
    /// 工作流任务页：流程节点表单处理
    /// </summary>
    WorkflowTask = 4,

    /// <summary>
    /// 数据看板：统计聚合展示（嵌入仪表盘 URL）
    /// </summary>
    Dashboard = 5,

    /// <summary>
    /// 自定义：指定前端组件路径，传入 DynamicConfig 作 props
    /// </summary>
    Custom = 6
}
