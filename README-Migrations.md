# 数据库迁移快速指南（MenuManagement）

本项目的数据库迁移方式参考了 `message-center-management`，使用 **MenuManagement.HttpApi.Host** 作为启动项目、**MenuManagement.EntityFrameworkCore** 作为迁移项目。

## 快速命令

### 创建初始迁移

```powershell
# Windows PowerShell
.\scripts\migrate-database.ps1 -MigrationName "InitialCreate"

# Linux/macOS
./scripts/migrate-database.sh --name InitialCreate
```

### 应用迁移

```powershell
# Windows PowerShell
.\scripts\migrate-database.ps1 -Update

# Linux/macOS
./scripts/migrate-database.sh --update
```

### 查看所有迁移

```powershell
.\scripts\migrate-database.ps1 -List
```

### 生成 SQL 脚本

```powershell
.\scripts\migrate-database.ps1 -Script
```

## 手动命令（不使用脚本时）

```bash
cd MenuManagement.HttpApi.Host

# 创建迁移
dotnet ef migrations add InitialCreate --project ../MenuManagement.EntityFrameworkCore

# 应用迁移
dotnet ef database update --project ../MenuManagement.EntityFrameworkCore
```

## 详细文档

请参阅 `docs/Database-Migrations.md` 获取完整的迁移指南和最佳实践说明。

