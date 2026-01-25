# 数据库迁移指南

## 概述

本文档基于参考项目 `message-center-management` 的实践，为 **MenuManagement** 提供一致的数据库迁移能力，使用 Entity Framework Core + ABP Framework。

## 项目结构（与迁移相关）

```text
MenuManagement/
├── MenuManagement.EntityFrameworkCore/
│   ├── MenuManagementDbContext.cs              # DbContext 定义
│   ├── MenuManagementDbContextFactory.cs       # 迁移工具使用的工厂类
│   └── Migrations/                             # 迁移文件目录（自动生成）
├── MenuManagement.HttpApi.Host/
│   ├── appsettings.json                        # 含数据库连接字符串
│   └── MenuManagement.HttpApi.Host.csproj      # 启动项目
├── scripts/
│   ├── migrate-database.ps1                    # Windows 迁移脚本
│   └── migrate-database.sh                     # Linux/macOS 迁移脚本
└── MenuManagement.sln
```

> 说明：迁移命令以 `MenuManagement.HttpApi.Host` 作为启动项目，以 `MenuManagement.EntityFrameworkCore` 作为迁移项目，完全对齐参考项目。

## 1. 配置数据库连接

编辑 `MenuManagement.HttpApi.Host/appsettings.json`：

```json
{
  "ConnectionStrings": {
    "Default": "User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=MenuManagement;Timezone=UTC;"
  }
}
```

> 已在当前项目中预置 PostgreSQL 连接字符串，如有需要请根据实际环境修改。

## 2. 创建初始迁移

### 2.1 使用脚本（推荐）

#### Windows (PowerShell)

```powershell
.\scripts\migrate-database.ps1 -MigrationName "InitialCreate"
```

#### Linux/macOS (Bash)

```bash
chmod +x scripts/migrate-database.sh
./scripts/migrate-database.sh --name InitialCreate
```

### 2.2 手动命令

```bash
cd MenuManagement.HttpApi.Host
dotnet ef migrations add InitialCreate --project ../MenuManagement.EntityFrameworkCore
```

迁移文件会生成到 `MenuManagement.EntityFrameworkCore/Migrations/` 目录下。

## 3. 应用迁移到数据库

### 3.1 使用脚本

```powershell
# Windows
.\scripts\migrate-database.ps1 -Update

# Linux/macOS
./scripts/migrate-database.sh --update
```

### 3.2 手动命令

```bash
cd MenuManagement.HttpApi.Host
dotnet ef database update --project ../MenuManagement.EntityFrameworkCore
```

## 4. 常用操作

### 4.1 创建新迁移

当你修改了实体类或 DbContext 配置后，需要创建新的迁移：

```powershell
# 使用脚本
.\scripts\migrate-database.ps1 -MigrationName "AddMenuTags"

# 手动命令
cd MenuManagement.HttpApi.Host
dotnet ef migrations add AddMenuTags --project ../MenuManagement.EntityFrameworkCore --startup-project .
```

### 4.2 查看所有迁移

```powershell
# 使用脚本
.\scripts\migrate-database.ps1 -List

# 手动命令
cd MenuManagement.HttpApi.Host
dotnet ef migrations list --project ../MenuManagement.EntityFrameworkCore --startup-project .
```

### 4.3 删除最后一个迁移

仅当该迁移尚未应用到数据库时才可以删除：

```powershell
# 使用脚本
.\scripts\migrate-database.ps1 -Remove

# 手动命令
cd MenuManagement.HttpApi.Host
dotnet ef migrations remove --project ../MenuManagement.EntityFrameworkCore --startup-project .
```

### 4.4 生成 SQL 脚本

生成迁移的 SQL 脚本，用于手动执行或审查：

```powershell
# 使用脚本
.\scripts\migrate-database.ps1 -Script

# 手动命令
cd MenuManagement.HttpApi.Host
dotnet ef migrations script --project ../MenuManagement.EntityFrameworkCore --startup-project . --output ../scripts/migrations/migration-script.sql
```

### 4.5 回滚到指定迁移

```bash
cd MenuManagement.HttpApi.Host
dotnet ef database update <迁移名称> --project ../MenuManagement.EntityFrameworkCore --startup-project .
```

示例：回滚到 `InitialCreate` 迁移：

```bash
dotnet ef database update InitialCreate --project ../MenuManagement.EntityFrameworkCore --startup-project .
```

### 4.6 应用所有待处理的迁移

```bash
cd MenuManagement.HttpApi.Host
dotnet ef database update --project ../MenuManagement.EntityFrameworkCore --startup-project .
```

## 5. 迁移文件说明

迁移文件位于 `MenuManagement.EntityFrameworkCore/Migrations/` 目录下，包含：

- `[时间戳]_[迁移名称].cs` - 迁移的 C# 代码
- `MenuManagementDbContextModelSnapshot.cs` - 当前数据库模型的快照

## 6. 最佳实践

### 6.1 迁移命名规范

使用描述性的迁移名称，清晰说明迁移的目的：

- ✅ `AddMenuOrganizationIndex`
- ✅ `UpdateMenuStatusEnum`
- ✅ `AddMenuRoleConstraints`
- ❌ `Migration1`
- ❌ `Update`

### 6.2 迁移前检查

- ✅ 实体和配置修改已完成
- ✅ 项目可以正常编译
- ✅ 生产库已备份

### 6.3 迁移审查

- ✅ 检查生成的迁移代码
- ✅ 在开发环境测试迁移
- ✅ 必要时生成 SQL 脚本并审查

### 6.4 生产环境部署建议

- ✅ 使用 SQL 脚本而非直接 `database update`
- ✅ 在非高峰时段执行
- ✅ 监控执行过程并准备回滚方案

## 7. 自动化与 CI/CD

在 CI/CD 流程中可自动应用迁移（示例）：

```yaml
- name: Apply Database Migrations
  run: |
    cd MenuManagement.HttpApi.Host
    dotnet ef database update --project ../MenuManagement.EntityFrameworkCore --startup-project . --no-build
```

> ⚠️ 生产环境仍建议由 DBA 或运维使用 SQL 脚本审核后执行。

