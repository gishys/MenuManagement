# 菜单管理系统

基于 ABP Framework 8.1.1 和 .NET 8.0 的完整菜单管理系统，包含用户、角色、组织、权限和菜单管理功能。

## 技术栈

- **.NET 8.0**
- **ABP Framework 8.1.1**
- **Entity Framework Core 8.0**
- **PostgreSQL** (支持 SQL Server)
- **Serilog** (日志记录)
- **Swagger/OpenAPI** (API 文档)

## 项目结构

```
MenuManagement/
├── MenuManagement.Domain.Shared/      # 领域共享层（枚举、常量）
├── MenuManagement.Domain/             # 领域层（实体、仓储接口）
├── MenuManagement.Application.Contracts/  # 应用契约层（DTO、服务接口）
├── MenuManagement.Application/        # 应用层（应用服务实现）
├── MenuManagement.EntityFrameworkCore/  # 数据访问层（EF Core 实现）
├── MenuManagement.HttpApi/             # HTTP API 层（控制器）
└── MenuManagement.HttpApi.Host/        # 启动项目
```

## 功能特性

### 1. 菜单管理
- ✅ 菜单的 CRUD 操作
- ✅ 树形菜单结构
- ✅ 菜单类型（目录、菜单、按钮）
- ✅ 菜单状态管理（启用/禁用）
- ✅ 菜单权限关联
- ✅ 根据角色/用户获取菜单

### 2. 组织管理
- ✅ 组织的 CRUD 操作
- ✅ 树形组织结构
- ✅ 组织类型（公司、部门、小组）
- ✅ 组织状态管理
- ✅ 组织负责人管理

### 3. 权限管理
- ✅ 基于 ABP PermissionManagement 模块
- ✅ 菜单权限定义
- ✅ 组织权限定义
- ✅ 角色权限分配

### 4. 用户和角色管理
- ✅ 基于 ABP Identity 模块
- ✅ 用户管理
- ✅ 角色管理
- ✅ 用户角色关联

## 数据库配置

项目默认使用 PostgreSQL，连接字符串在 `appsettings.json` 中配置：

```json
{
  "ConnectionStrings": {
    "Default": "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=MenuManagement;Timezone=UTC;"
  }
}
```

## 运行项目

1. **还原 NuGet 包**
   ```bash
   dotnet restore
   ```

2. **创建数据库迁移**
   ```bash
   cd MenuManagement.HttpApi.Host
   dotnet ef migrations add InitialCreate --project ../MenuManagement.EntityFrameworkCore
   ```

3. **更新数据库**
   ```bash
   dotnet ef database update --project ../MenuManagement.EntityFrameworkCore
   ```

4. **运行项目**
   ```bash
   dotnet run --project MenuManagement.HttpApi.Host
   ```

5. **访问 Swagger UI**
   - 打开浏览器访问: `http://localhost:5000/swagger`

## API 端点

### 菜单管理
- `GET /api/menus/tree` - 获取树形菜单列表
- `GET /api/menus/role/{roleId}` - 根据角色ID获取菜单
- `GET /api/menus/user/{userId}` - 根据用户ID获取菜单
- `POST /api/menus/role/{roleId}/assign` - 分配菜单给角色
- `PUT /api/menus/{id}/status` - 启用/禁用菜单

### 组织管理
- `GET /api/organizations/tree` - 获取树形组织列表
- `GET /api/organizations/user/{userId}` - 根据用户ID获取组织
- `PUT /api/organizations/{id}/status` - 启用/禁用组织

## 权限定义

系统定义了以下权限：

- `MenuManagement.Menus` - 菜单管理权限
  - `MenuManagement.Menus.Create` - 创建菜单
  - `MenuManagement.Menus.Update` - 更新菜单
  - `MenuManagement.Menus.Delete` - 删除菜单
  - `MenuManagement.Menus.View` - 查看菜单

- `MenuManagement.Organizations` - 组织管理权限
  - `MenuManagement.Organizations.Create` - 创建组织
  - `MenuManagement.Organizations.Update` - 更新组织
  - `MenuManagement.Organizations.Delete` - 删除组织
  - `MenuManagement.Organizations.View` - 查看组织

## 开发说明

### 添加新功能

1. **领域层**：在 `MenuManagement.Domain` 中添加实体和仓储接口
2. **应用契约层**：在 `MenuManagement.Application.Contracts` 中添加 DTO 和服务接口
3. **应用层**：在 `MenuManagement.Application` 中实现应用服务
4. **数据访问层**：在 `MenuManagement.EntityFrameworkCore` 中实现仓储
5. **API 层**：在 `MenuManagement.HttpApi` 中添加控制器

### 数据库迁移

```bash
# 创建迁移
dotnet ef migrations add MigrationName --project MenuManagement.EntityFrameworkCore --startup-project MenuManagement.HttpApi.Host

# 更新数据库
dotnet ef database update --project MenuManagement.EntityFrameworkCore --startup-project MenuManagement.HttpApi.Host
```

## 最佳实践

本项目遵循以下最佳实践：

1. **DDD（领域驱动设计）**：清晰的分层架构
2. **CQRS**：使用 ABP 的 CrudAppService 实现 CRUD 操作
3. **权限控制**：基于 ABP PermissionManagement 模块
4. **审计日志**：使用 ABP 的 FullAuditedEntity 自动记录创建、修改信息
5. **依赖注入**：使用 ABP 的模块化系统管理依赖

## 许可证

本项目基于 ABP Framework，遵循相应的许可证。
