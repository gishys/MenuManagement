# 数据库迁移脚本
# 用于创建和应用 Entity Framework Core 数据库迁移（MenuManagement）

param(
    [Parameter(Mandatory=$false)]
    [string]$MigrationName = "InitialCreate",
    
    [Parameter(Mandatory=$false)]
    [switch]$Update,
    
    [Parameter(Mandatory=$false)]
    [switch]$Remove,
    
    [Parameter(Mandatory=$false)]
    [switch]$List,
    
    [Parameter(Mandatory=$false)]
    [switch]$Script
)

$ErrorActionPreference = "Stop"

# 设置项目路径（对齐当前解决方案结构）
$ProjectPath = "MenuManagement.HttpApi.Host"
$DbContextProjectPath = "MenuManagement.EntityFrameworkCore"
$StartupProject = "$ProjectPath/MenuManagement.HttpApi.Host.csproj"
$DbContextProject = "$DbContextProjectPath/MenuManagement.EntityFrameworkCore.csproj"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "   MenuManagement 数据库迁移工具" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# 检查项目文件是否存在
if (-not (Test-Path $StartupProject)) {
    Write-Host "错误: 找不到启动项目: $StartupProject" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $DbContextProject)) {
    Write-Host "错误: 找不到 DbContext 项目: $DbContextProject" -ForegroundColor Red
    exit 1
}

# 切换到启动项目目录
Push-Location $ProjectPath

try {
    if ($List) {
        # 列出所有迁移
        Write-Host "列出所有数据库迁移..." -ForegroundColor Yellow
        dotnet ef migrations list --project "../$DbContextProjectPath" --startup-project .
        exit 0
    }
    
    if ($Remove) {
        # 删除最后一个迁移
        Write-Host "删除最后一个迁移..." -ForegroundColor Yellow
        dotnet ef migrations remove --project "../$DbContextProjectPath" --startup-project .
        exit 0
    }
    
    if ($Script) {
        # 生成 SQL 脚本
        Write-Host "生成迁移 SQL 脚本..." -ForegroundColor Yellow
        $scriptPath = "../scripts/migrations/migration-script.sql"
        New-Item -ItemType Directory -Force -Path "../scripts/migrations" | Out-Null
        dotnet ef migrations script --project "../$DbContextProjectPath" --startup-project . --output $scriptPath
        Write-Host "SQL 脚本已生成: $scriptPath" -ForegroundColor Green
        exit 0
    }
    
    if ($Update) {
        # 应用迁移到数据库
        Write-Host "应用数据库迁移..." -ForegroundColor Yellow
        dotnet ef database update --project "../$DbContextProjectPath" --startup-project .
        Write-Host "数据库迁移已成功应用!" -ForegroundColor Green
        exit 0
    }
    
    # 创建新迁移
    Write-Host "创建新的数据库迁移: $MigrationName" -ForegroundColor Yellow
    dotnet ef migrations add $MigrationName --project "../$DbContextProjectPath" --startup-project .
    Write-Host "迁移 '$MigrationName' 已成功创建!" -ForegroundColor Green
    Write-Host ""
    Write-Host "提示: 使用以下命令应用迁移到数据库:" -ForegroundColor Cyan
    Write-Host "  .\scripts\migrate-database.ps1 -Update" -ForegroundColor White
    
} catch {
    Write-Host "错误: $_" -ForegroundColor Red
    exit 1
} finally {
    Pop-Location
}

