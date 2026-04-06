-- =============================================================
-- Migration: V20260406__add_dynamic_menu_config.sql
-- Description: 菜单动态配置 - 新增功能类型、发布状态和动态配置字段
-- Applies to: MenuManagement (Menus table)
-- =============================================================

-- 新增菜单功能类型字段
-- 0=None(普通菜单), 1=DataQuery(查询分析), 2=DataManagement(数据管理),
-- 3=FormFill(表单填写), 4=WorkflowTask(工作流任务), 5=Dashboard(数据看板), 6=Custom(自定义)
ALTER TABLE "Menus"
    ADD COLUMN IF NOT EXISTS "FeatureType" integer NOT NULL DEFAULT 0;

-- 新增菜单发布状态字段（独立于 Status 启用/禁用）
-- 0=Draft(草稿), 1=Published(已发布), 2=Archived(已归档)
ALTER TABLE "Menus"
    ADD COLUMN IF NOT EXISTS "PublishStatus" integer NOT NULL DEFAULT 0;

-- 新增动态菜单配置 JSON 字段
-- 存储结构：{ entityTypeCode, queryFormCode, createFormCode, editFormCode,
--             viewFormCode, workflowFormCode, listColumns, defaultPageSize,
--             enableExport, enableImport, dashboardUrl, customComponentPath }
ALTER TABLE "Menus"
    ADD COLUMN IF NOT EXISTS "DynamicConfig" text;

-- 索引：加速按发布状态查询（GetPublishedTreeAsync 场景）
CREATE INDEX IF NOT EXISTS "IX_Menus_PublishStatus"
    ON "Menus" ("PublishStatus");

-- 索引：加速按功能类型查询
CREATE INDEX IF NOT EXISTS "IX_Menus_FeatureType"
    ON "Menus" ("FeatureType");

COMMENT ON COLUMN "Menus"."FeatureType" IS '菜单功能类型: 0=普通,1=查询分析,2=数据管理,3=表单填写,4=工作流任务,5=数据看板,6=自定义';
COMMENT ON COLUMN "Menus"."PublishStatus" IS '动态配置发布状态: 0=草稿,1=已发布,2=已归档';
COMMENT ON COLUMN "Menus"."DynamicConfig" IS '动态菜单配置(JSON): 表单绑定、列表列、导入导出等运行时配置';
