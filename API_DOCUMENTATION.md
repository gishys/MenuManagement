# 菜单管理 API 文档

## 目录

- [概述](#概述)
- [基础信息](#基础信息)
- [认证说明](#认证说明)
- [数据模型](#数据模型)
- [API 端点](#api-端点)
  - [菜单查询](#菜单查询)
  - [菜单管理](#菜单管理)
  - [菜单关联](#菜单关联)
- [错误码说明](#错误码说明)
- [示例代码](#示例代码)

---

## 概述

菜单管理 API 提供完整的菜单管理功能，包括菜单的创建、查询、更新、删除，以及菜单与角色、组织的关联管理。所有 API 均需要身份认证。

### 主要功能

- ✅ 菜单的 CRUD 操作
- ✅ 树形菜单结构查询
- ✅ 根据角色/用户/组织获取菜单
- ✅ 菜单与角色/组织的关联管理
- ✅ 菜单状态管理（启用/禁用）

---

## 基础信息

### 基础 URL

```
开发环境: http://localhost:5000
生产环境: {生产环境地址}
```

### API 版本

当前版本: `v1`

### 数据格式

- **请求格式**: `application/json`
- **响应格式**: `application/json`
- **字符编码**: `UTF-8`

### 时间格式

所有日期时间字段使用 ISO 8601 格式: `yyyy-MM-ddTHH:mm:ssZ`

---

## 认证说明

### JWT Token 认证

所有 API 请求都需要在请求头中携带 JWT Token：

```
Authorization: Bearer {your_access_token}
```

### 获取 Token

通过 ABP Identity 的认证接口获取访问令牌：

```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password&username={username}&password={password}&client_id={client_id}&client_secret={client_secret}
```

### Token 刷新

Token 过期后需要重新获取，或使用刷新令牌机制。

---

## 数据模型

### MenuDto - 菜单数据传输对象

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| id | Guid | 是 | 菜单唯一标识 |
| name | string | 是 | 菜单名称 |
| code | string | 是 | 菜单编码（唯一） |
| parentId | Guid? | 否 | 父菜单ID，为空表示根菜单 |
| parentName | string | 否 | 父菜单名称 |
| type | MenuType | 是 | 菜单类型：1-目录，2-菜单，3-按钮 |
| path | string | 否 | 路由路径 |
| component | string | 否 | 组件路径 |
| icon | string | 否 | 图标 |
| sort | int | 是 | 排序号 |
| status | MenuStatus | 是 | 状态：0-禁用，1-启用 |
| permission | string | 否 | 权限标识 |
| isHidden | bool | 是 | 是否隐藏 |
| isCache | bool | 是 | 是否缓存 |
| isExternal | bool | 是 | 是否外链 |
| externalUrl | string | 否 | 外链地址 |
| remark | string | 否 | 备注 |
| children | MenuDto[] | 否 | 子菜单列表 |
| creationTime | DateTime | 是 | 创建时间 |
| lastModificationTime | DateTime? | 否 | 最后修改时间 |
| creatorId | Guid? | 否 | 创建人ID |
| lastModifierId | Guid? | 否 | 最后修改人ID |

### CreateMenuDto - 创建菜单请求

| 字段名 | 类型 | 必填 | 说明 | 约束 |
|--------|------|------|------|------|
| name | string | 是 | 菜单名称 | 最大长度50 |
| code | string | 是 | 菜单编码 | 最大长度50，唯一 |
| parentId | Guid? | 否 | 父菜单ID | - |
| type | MenuType | 是 | 菜单类型 | 1-目录，2-菜单，3-按钮 |
| path | string | 否 | 路由路径 | 最大长度200 |
| component | string | 否 | 组件路径 | 最大长度200 |
| icon | string | 否 | 图标 | 最大长度50 |
| sort | int | 是 | 排序号 | - |
| status | MenuStatus | 否 | 状态 | 默认：1-启用 |
| permission | string | 否 | 权限标识 | 最大长度100 |
| isHidden | bool | 否 | 是否隐藏 | 默认：false |
| isCache | bool | 否 | 是否缓存 | 默认：false |
| isExternal | bool | 否 | 是否外链 | 默认：false |
| externalUrl | string | 否 | 外链地址 | 最大长度500 |
| remark | string | 否 | 备注 | 最大长度500 |

### UpdateMenuDto - 更新菜单请求

字段与 `CreateMenuDto` 相同，所有字段必填。

### PagedAndSortedResultRequestDto - 分页查询参数

| 字段名 | 类型 | 必填 | 说明 | 默认值 |
|--------|------|------|------|--------|
| skipCount | int | 否 | 跳过记录数 | 0 |
| maxResultCount | int | 否 | 每页最大记录数 | 10 |
| sorting | string | 否 | 排序字段 | "id" |

### PagedResultDto<T> - 分页响应

| 字段名 | 类型 | 说明 |
|--------|------|------|
| items | T[] | 数据列表 |
| totalCount | long | 总记录数 |

### 枚举类型

#### MenuType - 菜单类型

```json
{
  "Directory": 1,  // 目录
  "Menu": 2,       // 菜单
  "Button": 3      // 按钮
}
```

#### MenuStatus - 菜单状态

```json
{
  "Disabled": 0,  // 禁用
  "Enabled": 1  // 启用
}
```

---

## API 端点

### 菜单查询

#### 1. 获取树形菜单列表

获取所有菜单的树形结构，按排序字段排序。

**请求**

```http
GET /api/menus/tree
Authorization: Bearer {token}
```

**响应**

直接返回菜单列表数组（ABP框架自动处理为200状态码）：

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "系统管理",
    "code": "system",
    "parentId": null,
    "parentName": null,
    "type": 1,
    "path": "/system",
    "component": "Layout",
    "icon": "system",
    "sort": 1,
    "status": 1,
    "permission": "system:view",
    "isHidden": false,
    "isCache": true,
    "isExternal": false,
    "externalUrl": null,
    "remark": "系统管理模块",
    "children": [
      {
        "id": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
        "name": "用户管理",
        "code": "system:user",
        "parentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "parentName": "系统管理",
        "type": 2,
        "path": "/system/user",
        "component": "system/user/index",
        "icon": "user",
        "sort": 1,
        "status": 1,
        "permission": "system:user:view",
        "isHidden": false,
        "isCache": true,
        "isExternal": false,
        "externalUrl": null,
        "remark": null,
        "children": []
      }
    ],
    "creationTime": "2024-01-01T00:00:00Z",
    "lastModificationTime": null,
    "creatorId": null,
    "lastModifierId": null
  }
]
```

> **注意**: ABP框架会自动将返回值包装为合适的HTTP响应，无需手动使用`Ok()`等方法。

**状态码**

- `200 OK` - 成功返回菜单树
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

#### 2. 获取分页菜单列表

获取菜单的分页列表，支持排序和筛选。

**请求**

```http
GET /api/menus?skipCount=0&maxResultCount=10&sorting=sort
Authorization: Bearer {token}
```

**查询参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| skipCount | int | 否 | 跳过记录数，默认0 |
| maxResultCount | int | 否 | 每页最大记录数，默认10 |
| sorting | string | 否 | 排序字段，默认"id" |

**响应**

直接返回分页结果对象（ABP框架自动处理为200状态码）：

```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "系统管理",
      "code": "system",
      "type": 1,
      "sort": 1,
      "status": 1,
      "children": []
    }
  ],
  "totalCount": 100
}
```

**状态码**

- `200 OK` - 成功返回菜单列表
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

#### 3. 根据ID获取菜单

获取指定ID的菜单详细信息。

**请求**

```http
GET /api/menus/{id}
Authorization: Bearer {token}
```

**路径参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| id | Guid | 是 | 菜单ID |

**响应**

直接返回菜单对象（ABP框架自动处理为200状态码）：

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "系统管理",
  "code": "system",
  "parentId": null,
  "parentName": null,
  "type": 1,
  "path": "/system",
  "component": "Layout",
  "icon": "system",
  "sort": 1,
  "status": 1,
  "permission": "system:view",
  "isHidden": false,
  "isCache": true,
  "isExternal": false,
  "externalUrl": null,
  "remark": "系统管理模块",
  "children": [],
  "creationTime": "2024-01-01T00:00:00Z",
  "lastModificationTime": null,
  "creatorId": null,
  "lastModifierId": null
}
```

**状态码**

- `200 OK` - 成功返回菜单信息
- `400 Bad Request` - 菜单ID无效
- `404 Not Found` - 菜单不存在
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

#### 4. 根据角色ID获取菜单

获取指定角色关联的所有菜单，返回树形结构。

**请求**

```http
GET /api/menus/role/{roleId}
Authorization: Bearer {token}
```

**路径参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| roleId | Guid | 是 | 角色ID |

**响应**

直接返回菜单列表数组（ABP框架自动处理为200状态码），格式与"获取树形菜单列表"相同，但只返回该角色关联的菜单。

**状态码**

- `200 OK` - 成功返回菜单树
- `400 Bad Request` - 角色ID无效
- `404 Not Found` - 角色不存在
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

#### 5. 根据用户ID获取菜单

获取指定用户通过角色关联的所有菜单，返回树形结构。

**请求**

```http
GET /api/menus/user/{userId}
Authorization: Bearer {token}
```

**路径参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| userId | Guid | 是 | 用户ID |

**响应**

直接返回菜单列表数组（ABP框架自动处理为200状态码），格式与"获取树形菜单列表"相同，但只返回该用户可访问的菜单。

**状态码**

- `200 OK` - 成功返回菜单树
- `400 Bad Request` - 用户ID无效
- `404 Not Found` - 用户不存在
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

#### 6. 根据组织ID获取菜单

获取指定组织关联的所有菜单，返回树形结构。

**请求**

```http
GET /api/menus/organization/{organizationId}
Authorization: Bearer {token}
```

**路径参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| organizationId | Guid | 是 | 组织单元ID（ABP Identity OrganizationUnit Id） |

**响应**

直接返回菜单列表数组（ABP框架自动处理为200状态码），格式与"获取树形菜单列表"相同，但只返回该组织关联的菜单。

**状态码**

- `200 OK` - 成功返回菜单树
- `400 Bad Request` - 组织ID无效
- `404 Not Found` - 组织不存在
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

### 菜单管理

#### 7. 创建菜单

创建一个新的菜单项。

**请求**

```http
POST /api/menus
Authorization: Bearer {token}
Content-Type: application/json
```

**请求体**

```json
{
  "name": "系统管理",
  "code": "system",
  "parentId": null,
  "type": 1,
  "path": "/system",
  "component": "Layout",
  "icon": "system",
  "sort": 1,
  "status": 1,
  "permission": "system:view",
  "isHidden": false,
  "isCache": true,
  "isExternal": false,
  "externalUrl": null,
  "remark": "系统管理模块"
}
```

**响应**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "系统管理",
  "code": "system",
  "parentId": null,
  "type": 1,
  "path": "/system",
  "component": "Layout",
  "icon": "system",
  "sort": 1,
  "status": 1,
  "permission": "system:view",
  "isHidden": false,
  "isCache": true,
  "isExternal": false,
  "externalUrl": null,
  "remark": "系统管理模块",
  "children": [],
  "creationTime": "2024-01-01T00:00:00Z",
  "lastModificationTime": null,
  "creatorId": null,
  "lastModifierId": null
}
```

**状态码**

- `201 Created` - 创建成功
- `400 Bad Request` - 请求参数无效
- `409 Conflict` - 菜单编码已存在
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

#### 8. 更新菜单

更新指定菜单的信息。

**请求**

```http
PUT /api/menus/{id}
Authorization: Bearer {token}
Content-Type: application/json
```

**路径参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| id | Guid | 是 | 菜单ID |

**请求体**

```json
{
  "name": "系统管理（已更新）",
  "code": "system",
  "parentId": null,
  "type": 1,
  "path": "/system",
  "component": "Layout",
  "icon": "system",
  "sort": 1,
  "status": 1,
  "permission": "system:view",
  "isHidden": false,
  "isCache": true,
  "isExternal": false,
  "externalUrl": null,
  "remark": "系统管理模块（已更新）"
}
```

**响应**

直接返回更新后的菜单对象（ABP框架自动处理为200状态码），格式与"根据ID获取菜单"相同。

**状态码**

- `200 OK` - 更新成功
- `400 Bad Request` - 请求参数无效
- `404 Not Found` - 菜单不存在
- `409 Conflict` - 菜单编码已存在
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

#### 9. 删除菜单

删除指定ID的菜单。如果菜单有子菜单，需要先删除子菜单。

**请求**

```http
DELETE /api/menus/{id}
Authorization: Bearer {token}
```

**路径参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| id | Guid | 是 | 菜单ID |

**响应**

无响应体，HTTP状态码204表示删除成功（ABP框架自动处理）。

**状态码**

- `204 No Content` - 删除成功
- `400 Bad Request` - 请求参数无效
- `404 Not Found` - 菜单不存在
- `409 Conflict` - 菜单存在子菜单，无法删除
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

#### 10. 启用/禁用菜单

设置菜单的启用或禁用状态。禁用的菜单不会在查询结果中返回。

**请求**

```http
PUT /api/menus/{id}/status?enabled=true
Authorization: Bearer {token}
```

**路径参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| id | Guid | 是 | 菜单ID |

**查询参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| enabled | bool | 是 | 是否启用（true=启用，false=禁用） |

**响应**

无响应体，HTTP状态码200表示成功（ABP框架自动处理）。

**状态码**

- `200 OK` - 操作成功
- `400 Bad Request` - 请求参数无效
- `404 Not Found` - 菜单不存在
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

### 菜单关联

#### 11. 分配菜单给角色

为指定角色分配菜单权限。会清除该角色原有的菜单关联，然后设置新的菜单关联。

**请求**

```http
POST /api/menus/role/{roleId}/assign
Authorization: Bearer {token}
Content-Type: application/json
```

**路径参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| roleId | Guid | 是 | 角色ID |

**请求体**

```json
{
  "menuIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "4fa85f64-5717-4562-b3fc-2c963f66afa7"
  ]
}
```

**响应**

无响应体，HTTP状态码200表示成功（ABP框架自动处理）。

**状态码**

- `200 OK` - 分配成功
- `400 Bad Request` - 请求参数无效（角色ID为空、菜单ID列表为空或包含无效ID）
- `404 Not Found` - 角色或菜单不存在
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

#### 12. 分配菜单给组织

为指定组织分配菜单权限。会清除该组织原有的菜单关联，然后设置新的菜单关联。

**请求**

```http
POST /api/menus/organization/{organizationId}/assign
Authorization: Bearer {token}
Content-Type: application/json
```

**路径参数**

| 参数名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| organizationId | Guid | 是 | 组织单元ID（ABP Identity OrganizationUnit Id） |

**请求体**

```json
{
  "menuIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "4fa85f64-5717-4562-b3fc-2c963f66afa7"
  ]
}
```

**响应**

无响应体，HTTP状态码200表示成功（ABP框架自动处理）。

**状态码**

- `200 OK` - 分配成功
- `400 Bad Request` - 请求参数无效（组织ID为空、菜单ID列表为空或包含无效ID）
- `404 Not Found` - 组织或菜单不存在
- `401 Unauthorized` - 未授权
- `403 Forbidden` - 权限不足

---

## 错误码说明

### HTTP 状态码

| 状态码 | 说明 | 处理建议 |
|--------|------|----------|
| 200 | 请求成功 | - |
| 201 | 创建成功 | - |
| 204 | 删除成功，无响应体 | - |
| 400 | 请求参数无效 | 检查请求参数格式和必填项 |
| 401 | 未授权 | 检查 Token 是否有效或已过期 |
| 403 | 权限不足 | 检查用户是否有相应权限 |
| 404 | 资源不存在 | 检查资源ID是否正确 |
| 409 | 资源冲突 | 检查资源是否已存在（如编码重复） |
| 500 | 服务器内部错误 | 联系技术支持 |

### 错误响应格式

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-1234567890abcdef-1234567890abcdef-01",
  "errors": {
    "name": [
      "The name field is required."
    ],
    "code": [
      "The code field is required."
    ]
  }
}
```

---

## 示例代码

### JavaScript/TypeScript (Axios)

```javascript
import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000';
const token = 'your_access_token';

// 创建 axios 实例
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  }
});

// 获取树形菜单列表
async function getMenuTree() {
  try {
    const response = await apiClient.get('/api/menus/tree');
    return response.data;
  } catch (error) {
    console.error('获取菜单树失败:', error.response?.data || error.message);
    throw error;
  }
}

// 创建菜单
async function createMenu(menuData) {
  try {
    const response = await apiClient.post('/api/menus', menuData);
    return response.data;
  } catch (error) {
    console.error('创建菜单失败:', error.response?.data || error.message);
    throw error;
  }
}

// 更新菜单
async function updateMenu(menuId, menuData) {
  try {
    const response = await apiClient.put(`/api/menus/${menuId}`, menuData);
    return response.data;
  } catch (error) {
    console.error('更新菜单失败:', error.response?.data || error.message);
    throw error;
  }
}

// 删除菜单
async function deleteMenu(menuId) {
  try {
    await apiClient.delete(`/api/menus/${menuId}`);
    return true;
  } catch (error) {
    console.error('删除菜单失败:', error.response?.data || error.message);
    throw error;
  }
}

// 根据角色ID获取菜单
async function getMenusByRoleId(roleId) {
  try {
    const response = await apiClient.get(`/api/menus/role/${roleId}`);
    return response.data;
  } catch (error) {
    console.error('获取角色菜单失败:', error.response?.data || error.message);
    throw error;
  }
}

// 分配菜单给角色
async function assignMenusToRole(roleId, menuIds) {
  try {
    await apiClient.post(`/api/menus/role/${roleId}/assign`, {
      menuIds: menuIds
    });
    // 成功时无响应体，状态码200表示成功
  } catch (error) {
    console.error('分配菜单失败:', error.response?.data || error.message);
    throw error;
  }
}

// 启用/禁用菜单
async function setMenuStatus(menuId, enabled) {
  try {
    await apiClient.put(`/api/menus/${menuId}/status`, null, {
      params: { enabled }
    });
    // 成功时无响应体，状态码200表示成功
  } catch (error) {
    console.error('设置菜单状态失败:', error.response?.data || error.message);
    throw error;
  }
}

// 使用示例
(async () => {
  // 获取菜单树
  const menuTree = await getMenuTree();
  console.log('菜单树:', menuTree);

  // 创建菜单
  const newMenu = await createMenu({
    name: '新菜单',
    code: 'new-menu',
    type: 2,
    path: '/new-menu',
    component: 'new-menu/index',
    sort: 1,
    status: 1
  });
  console.log('创建的菜单:', newMenu);

  // 更新菜单
  const updatedMenu = await updateMenu(newMenu.id, {
    ...newMenu,
    name: '更新的菜单名称'
  });
  console.log('更新的菜单:', updatedMenu);

  // 分配菜单给角色（无返回值，状态码200表示成功）
  await assignMenusToRole('role-id', [newMenu.id]);
  console.log('菜单分配成功');

  // 启用菜单（无返回值，状态码200表示成功）
  await setMenuStatus(newMenu.id, true);
  console.log('菜单已启用');
})();
```

### C# (.NET)

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class MenuApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public MenuApiClient(string baseUrl, string token)
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    // 获取树形菜单列表
    public async Task<List<MenuDto>> GetMenuTreeAsync()
    {
        var response = await _httpClient.GetAsync("/api/menus/tree");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<MenuDto>>(json);
    }

    // 创建菜单
    public async Task<MenuDto> CreateMenuAsync(CreateMenuDto menu)
    {
        var json = JsonSerializer.Serialize(menu);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/api/menus", content);
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MenuDto>(responseJson);
    }

    // 更新菜单
    public async Task<MenuDto> UpdateMenuAsync(Guid menuId, UpdateMenuDto menu)
    {
        var json = JsonSerializer.Serialize(menu);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"/api/menus/{menuId}", content);
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MenuDto>(responseJson);
    }

    // 删除菜单
    public async Task DeleteMenuAsync(Guid menuId)
    {
        var response = await _httpClient.DeleteAsync($"/api/menus/{menuId}");
        response.EnsureSuccessStatusCode();
    }

    // 根据角色ID获取菜单
    public async Task<List<MenuDto>> GetMenusByRoleIdAsync(Guid roleId)
    {
        var response = await _httpClient.GetAsync($"/api/menus/role/{roleId}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<MenuDto>>(json);
    }

    // 分配菜单给角色
    public async Task AssignMenusToRoleAsync(Guid roleId, List<Guid> menuIds)
    {
        var json = JsonSerializer.Serialize(new { menuIds });
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(
            $"/api/menus/role/{roleId}/assign", content);
        response.EnsureSuccessStatusCode();
    }

    // 启用/禁用菜单
    public async Task SetMenuStatusAsync(Guid menuId, bool enabled)
    {
        var response = await _httpClient.PutAsync(
            $"/api/menus/{menuId}/status?enabled={enabled}", null);
        response.EnsureSuccessStatusCode();
    }
}

// 使用示例
class Program
{
    static async Task Main(string[] args)
    {
        var client = new MenuApiClient("http://localhost:5000", "your_token");

        // 获取菜单树
        var menuTree = await client.GetMenuTreeAsync();
        Console.WriteLine($"获取到 {menuTree.Count} 个根菜单");

        // 创建菜单
        var newMenu = await client.CreateMenuAsync(new CreateMenuDto
        {
            Name = "新菜单",
            Code = "new-menu",
            Type = MenuType.Menu,
            Path = "/new-menu",
            Component = "new-menu/index",
            Sort = 1,
            Status = MenuStatus.Enabled
        });
        Console.WriteLine($"创建菜单成功，ID: {newMenu.Id}");

        // 更新菜单
        var updatedMenu = await client.UpdateMenuAsync(newMenu.Id, new UpdateMenuDto
        {
            Name = "更新的菜单名称",
            Code = newMenu.Code,
            Type = newMenu.Type,
            Sort = newMenu.Sort,
            Status = newMenu.Status
        });
        Console.WriteLine($"更新菜单成功");

    // 分配菜单给角色（无返回值，状态码200表示成功）
    await client.AssignMenusToRoleAsync(
        Guid.Parse("role-id"), 
        new List<Guid> { newMenu.Id });
    Console.WriteLine("菜单分配成功");

    // 启用菜单（无返回值，状态码200表示成功）
    await client.SetMenuStatusAsync(newMenu.Id, true);
    Console.WriteLine("菜单已启用");
    }
}
```

### cURL

```bash
# 获取树形菜单列表
curl -X GET "http://localhost:5000/api/menus/tree" \
  -H "Authorization: Bearer your_access_token" \
  -H "Content-Type: application/json"

# 创建菜单
curl -X POST "http://localhost:5000/api/menus" \
  -H "Authorization: Bearer your_access_token" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "系统管理",
    "code": "system",
    "type": 1,
    "path": "/system",
    "component": "Layout",
    "icon": "system",
    "sort": 1,
    "status": 1
  }'

# 更新菜单
curl -X PUT "http://localhost:5000/api/menus/{menuId}" \
  -H "Authorization: Bearer your_access_token" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "系统管理（已更新）",
    "code": "system",
    "type": 1,
    "path": "/system",
    "component": "Layout",
    "icon": "system",
    "sort": 1,
    "status": 1
  }'

# 删除菜单
curl -X DELETE "http://localhost:5000/api/menus/{menuId}" \
  -H "Authorization: Bearer your_access_token"

# 根据角色ID获取菜单
curl -X GET "http://localhost:5000/api/menus/role/{roleId}" \
  -H "Authorization: Bearer your_access_token" \
  -H "Content-Type: application/json"

# 分配菜单给角色
curl -X POST "http://localhost:5000/api/menus/role/{roleId}/assign" \
  -H "Authorization: Bearer your_access_token" \
  -H "Content-Type: application/json" \
  -d '{
    "menuIds": [
      "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "4fa85f64-5717-4562-b3fc-2c963f66afa7"
    ]
  }'

# 启用/禁用菜单
curl -X PUT "http://localhost:5000/api/menus/{menuId}/status?enabled=true" \
  -H "Authorization: Bearer your_access_token"
```

---

## 最佳实践

### 1. 错误处理

- 始终检查 HTTP 状态码
- 处理网络错误和超时
- 记录错误日志以便调试

### 2. 性能优化

- 使用分页查询避免一次性加载大量数据
- 缓存菜单树结构，减少重复请求
- 使用树形结构查询而非多次单独查询

### 3. 安全性

- 妥善保管 Token，不要在前端代码中硬编码
- Token 过期后及时刷新
- 使用 HTTPS 传输敏感数据

### 4. 数据验证

- 在客户端进行基础验证，减少无效请求
- 服务端验证是最终保障，不要依赖客户端验证

### 5. 菜单树构建

前端可以根据返回的菜单列表自行构建树形结构，或直接使用服务端返回的树形结构。

---

## 更新日志

### v1.0.0 (2024-01-01)

- 初始版本发布
- 支持菜单的完整 CRUD 操作
- 支持菜单与角色、组织的关联管理
- 支持树形菜单结构查询

---

## 联系方式

如有问题或建议，请联系：

- **技术支持**: support@menumanagement.com
- **API 文档**: http://localhost:5000/swagger
- **项目地址**: {项目地址}

---

**文档版本**: 1.0.0  
**最后更新**: 2024-01-01
