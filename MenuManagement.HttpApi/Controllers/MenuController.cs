using MenuManagement.Application.Contracts.DTOs;
using MenuManagement.Application.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace MenuManagement.HttpApi.Controllers;

/// <summary>
/// 菜单管理控制器
/// 提供菜单的创建、查询、更新、删除以及菜单与角色、组织的关联管理功能
/// </summary>
/// <remarks>
/// 构造函数
/// </remarks>
/// <param name="menuAppService">菜单应用服务</param>
[ApiController]
[Authorize]
[Route("api/menus")]
[Produces("application/json")]
[ProducesResponseType(401)]
[ProducesResponseType(403)]
public class MenuController(IMenuAppService menuAppService) : AbpControllerBase
{
    private readonly IMenuAppService _menuAppService = menuAppService;

    /// <summary>
    /// 获取树形菜单列表
    /// 返回所有菜单的树形结构，按排序字段排序
    /// </summary>
    /// <returns>树形菜单列表</returns>
    /// <response code="200">成功返回菜单树</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    [HttpGet("tree")]
    [ProducesResponseType(200, Type = typeof(List<MenuDto>))]
    public async Task<List<MenuDto>> GetTreeAsync()
    {
        return await _menuAppService.GetTreeAsync();
    }

    /// <summary>
    /// 根据角色ID获取菜单
    /// 获取指定角色关联的所有菜单，返回树形结构
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <returns>该角色关联的菜单树</returns>
    /// <response code="200">成功返回菜单树</response>
    /// <response code="400">角色ID无效</response>
    /// <response code="404">角色不存在</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    [HttpGet("role/{roleId}")]
    [ProducesResponseType(200, Type = typeof(List<MenuDto>))]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(404, Type = typeof(ProblemDetails))]
    public async Task<List<MenuDto>> GetMenusByRoleIdAsync([Required] Guid roleId)
    {
        return await _menuAppService.GetMenusByRoleIdAsync(roleId);
    }

    /// <summary>
    /// 根据用户ID获取菜单
    /// 获取指定用户通过角色关联的所有菜单，返回树形结构
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>该用户可访问的菜单树</returns>
    /// <response code="200">成功返回菜单树</response>
    /// <response code="400">用户ID无效</response>
    /// <response code="404">用户不存在</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(200, Type = typeof(List<MenuDto>))]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(404, Type = typeof(ProblemDetails))]
    public async Task<List<MenuDto>> GetMenusByUserIdAsync([Required] Guid userId)
    {
        return await _menuAppService.GetMenusByUserIdAsync(userId);
    }

    /// <summary>
    /// 根据组织ID获取菜单
    /// 获取指定组织关联的所有菜单，返回树形结构
    /// </summary>
    /// <param name="organizationId">组织单元ID（ABP Identity OrganizationUnit Id）</param>
    /// <returns>该组织关联的菜单树</returns>
    /// <response code="200">成功返回菜单树</response>
    /// <response code="400">组织ID无效</response>
    /// <response code="404">组织不存在</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    [HttpGet("organization/{organizationId}")]
    [ProducesResponseType(200, Type = typeof(List<MenuDto>))]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(404, Type = typeof(ProblemDetails))]
    public async Task<List<MenuDto>> GetMenusByOrganizationIdAsync([Required] Guid organizationId)
    {
        return await _menuAppService.GetMenusByOrganizationIdAsync(organizationId);
    }

    /// <summary>
    /// 分配菜单给角色
    /// 为指定角色分配菜单权限，会清除该角色原有的菜单关联，然后设置新的菜单关联
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="menuIds">菜单ID列表</param>
    /// <response code="200">分配成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="404">角色或菜单不存在</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    /// <remarks>
    /// 示例请求:
    /// 
    ///     POST /api/menus/role/{roleId}/assign
    ///     {
    ///       "menuIds": [
    ///         "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "4fa85f64-5717-4562-b3fc-2c963f66afa7"
    ///       ]
    ///     }
    /// </remarks>
    [HttpPost("role/{roleId}/assign")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(404, Type = typeof(ProblemDetails))]
    public async Task AssignMenusToRoleAsync(
        [Required] Guid roleId,
        [FromBody, Required] List<Guid> menuIds)
    {
        await _menuAppService.AssignMenusToRoleAsync(roleId, menuIds);
    }

    /// <summary>
    /// 分配菜单给组织
    /// 为指定组织分配菜单权限，会清除该组织原有的菜单关联，然后设置新的菜单关联
    /// </summary>
    /// <param name="organizationId">组织单元ID（ABP Identity OrganizationUnit Id）</param>
    /// <param name="menuIds">菜单ID列表</param>
    /// <response code="200">分配成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="404">组织或菜单不存在</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    /// <remarks>
    /// 示例请求:
    /// 
    ///     POST /api/menus/organization/{organizationId}/assign
    ///     {
    ///       "menuIds": [
    ///         "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "4fa85f64-5717-4562-b3fc-2c963f66afa7"
    ///       ]
    ///     }
    /// </remarks>
    [HttpPost("organization/{organizationId}/assign")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(404, Type = typeof(ProblemDetails))]
    public async Task AssignMenusToOrganizationAsync(
        [Required] Guid organizationId,
        [FromBody, Required] List<Guid> menuIds)
    {
        await _menuAppService.AssignMenusToOrganizationAsync(organizationId, menuIds);
    }

    /// <summary>
    /// 启用/禁用菜单
    /// 设置菜单的启用或禁用状态，禁用的菜单不会在查询结果中返回
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="enabled">是否启用（true=启用，false=禁用）</param>
    /// <response code="200">操作成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="404">菜单不存在</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    /// <remarks>
    /// 示例请求:
    /// 
    ///     PUT /api/menus/{id}/status?enabled=true
    /// </remarks>
    [HttpPut("{id}/status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(404, Type = typeof(ProblemDetails))]
    public async Task SetStatusAsync([Required] Guid id, [FromQuery, Required] bool enabled)
    {
        await _menuAppService.SetStatusAsync(id, enabled);
    }

    /// <summary>
    /// 创建菜单
    /// 创建一个新的菜单项
    /// </summary>
    /// <param name="input">菜单创建信息</param>
    /// <returns>创建的菜单信息</returns>
    /// <response code="201">创建成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    /// <response code="409">菜单编码已存在</response>
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(MenuDto))]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(409, Type = typeof(ProblemDetails))]
    public async Task<MenuDto> CreateAsync([FromBody, Required] CreateMenuDto input)
    {
        return await _menuAppService.CreateAsync(input);
    }

    /// <summary>
    /// 根据ID获取菜单
    /// 获取指定ID的菜单详细信息
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>菜单信息</returns>
    /// <response code="200">成功返回菜单信息</response>
    /// <response code="404">菜单不存在</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    [HttpGet("{id}")]
    [ProducesResponseType(200, Type = typeof(MenuDto))]
    [ProducesResponseType(404, Type = typeof(ProblemDetails))]
    public async Task<MenuDto> GetByIdAsync([Required] Guid id)
    {
        return await _menuAppService.GetAsync(id);
    }

    /// <summary>
    /// 更新菜单
    /// 更新指定菜单的信息
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="input">菜单更新信息</param>
    /// <returns>更新后的菜单信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="404">菜单不存在</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    /// <response code="409">菜单编码已存在</response>
    [HttpPut("{id}")]
    [ProducesResponseType(200, Type = typeof(MenuDto))]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(404, Type = typeof(ProblemDetails))]
    [ProducesResponseType(409, Type = typeof(ProblemDetails))]
    public async Task<MenuDto> UpdateAsync([Required] Guid id, [FromBody, Required] UpdateMenuDto input)
    {
        return await _menuAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除菜单
    /// 删除指定ID的菜单，如果菜单有子菜单，需要先删除子菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <response code="204">删除成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="404">菜单不存在</response>
    /// <response code="409">菜单存在子菜单，无法删除</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(404, Type = typeof(ProblemDetails))]
    [ProducesResponseType(409, Type = typeof(ProblemDetails))]
    public async Task DeleteAsync([Required] Guid id)
    {
        await _menuAppService.DeleteAsync(id);
    }

    /// <summary>
    /// 获取菜单列表（分页）
    /// 获取菜单的分页列表，支持排序和筛选
    /// </summary>
    /// <param name="input">分页和排序参数</param>
    /// <returns>分页菜单列表</returns>
    /// <response code="200">成功返回菜单列表</response>
    /// <response code="401">未授权</response>
    /// <response code="403">权限不足</response>
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(PagedResultDto<MenuDto>))]
    public async Task<PagedResultDto<MenuDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return await _menuAppService.GetListAsync(input);
    }
}
