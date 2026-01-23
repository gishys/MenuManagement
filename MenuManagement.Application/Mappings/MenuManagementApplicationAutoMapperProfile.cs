using AutoMapper;
using MenuManagement.Application.Contracts.DTOs;
using MenuManagement.Domain.Entities;

namespace MenuManagement.Application.Mappings;

/// <summary>
/// 菜单管理应用层AutoMapper配置
/// </summary>
public class MenuManagementApplicationAutoMapperProfile : Profile
{
    public MenuManagementApplicationAutoMapperProfile()
    {
        CreateMap<Menu, MenuDto>();
    }
}
