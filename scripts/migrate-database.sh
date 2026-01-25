#!/bin/bash
# 数据库迁移脚本
# 用于创建和应用 Entity Framework Core 数据库迁移（MenuManagement）

set -e

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# 默认参数
MIGRATION_NAME="InitialCreate"
ACTION="add"

# 解析参数
while [[ $# -gt 0 ]]; do
    case $1 in
        -n|--name)
            MIGRATION_NAME="$2"
            shift 2
            ;;
        -u|--update)
            ACTION="update"
            shift
            ;;
        -r|--remove)
            ACTION="remove"
            shift
            ;;
        -l|--list)
            ACTION="list"
            shift
            ;;
        -s|--script)
            ACTION="script"
            shift
            ;;
        *)
            echo -e "${RED}未知参数: $1${NC}"
            exit 1
            ;;
    esac
done

# 设置项目路径（对齐当前解决方案结构）
PROJECT_PATH="MenuManagement.HttpApi.Host"
DBCONTEXT_PROJECT_PATH="MenuManagement.EntityFrameworkCore"
STARTUP_PROJECT="$PROJECT_PATH/MenuManagement.HttpApi.Host.csproj"
DBCONTEXT_PROJECT="$DBCONTEXT_PROJECT_PATH/MenuManagement.EntityFrameworkCore.csproj"

echo -e "${CYAN}=========================================${NC}"
echo -e "${CYAN}   MenuManagement 数据库迁移工具${NC}"
echo -e "${CYAN}=========================================${NC}"
echo ""

# 检查项目文件是否存在
if [ ! -f "$STARTUP_PROJECT" ]; then
    echo -e "${RED}错误: 找不到启动项目: $STARTUP_PROJECT${NC}"
    exit 1
fi

if [ ! -f "$DBCONTEXT_PROJECT" ]; then
    echo -e "${RED}错误: 找不到 DbContext 项目: $DBCONTEXT_PROJECT${NC}"
    exit 1
fi

# 切换到启动项目目录
cd "$PROJECT_PATH"

# 执行相应操作
case $ACTION in
    list)
        echo -e "${YELLOW}列出所有数据库迁移...${NC}"
        dotnet ef migrations list --project "../$DBCONTEXT_PROJECT_PATH" --startup-project .
        ;;
    remove)
        echo -e "${YELLOW}删除最后一个迁移...${NC}"
        dotnet ef migrations remove --project "../$DBCONTEXT_PROJECT_PATH" --startup-project .
        ;;
    script)
        echo -e "${YELLOW}生成迁移 SQL 脚本...${NC}"
        mkdir -p "../scripts/migrations"
        SCRIPT_PATH="../scripts/migrations/migration-script.sql"
        dotnet ef migrations script --project "../$DBCONTEXT_PROJECT_PATH" --startup-project . --output "$SCRIPT_PATH"
        echo -e "${GREEN}SQL 脚本已生成: $SCRIPT_PATH${NC}"
        ;;
    update)
        echo -e "${YELLOW}应用数据库迁移...${NC}"
        dotnet ef database update --project "../$DBCONTEXT_PROJECT_PATH" --startup-project .
        echo -e "${GREEN}数据库迁移已成功应用!${NC}"
        ;;
    add)
        echo -e "${YELLOW}创建新的数据库迁移: $MIGRATION_NAME${NC}"
        dotnet ef migrations add "$MIGRATION_NAME" --project "../$DBCONTEXT_PROJECT_PATH" --startup-project .
        echo -e "${GREEN}迁移 '$MIGRATION_NAME' 已成功创建!${NC}"
        echo ""
        echo -e "${CYAN}提示: 使用以下命令应用迁移到数据库:${NC}"
        echo -e "${NC}  ./scripts/migrate-database.sh --update${NC}"
        ;;
esac

