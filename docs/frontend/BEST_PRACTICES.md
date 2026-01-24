# 前端开发最佳实践文档

> 本文档为使用 Cursor AI 创建菜单管理前端项目提供指导，基于后端 API 设计最佳实践。

## 目录

- [项目结构](#项目结构)
- [技术栈建议](#技术栈建议)
- [API 集成](#api-集成)
- [类型定义](#类型定义)
- [状态管理](#状态管理)
- [错误处理](#错误处理)
- [性能优化](#性能优化)
- [安全性](#安全性)
- [代码规范](#代码规范)
- [测试策略](#测试策略)
- [常见问题](#常见问题)

---

## 项目结构

### 推荐的目录结构

```
frontend/
├── src/
│   ├── api/                    # API 调用层
│   │   ├── client.ts          # HTTP 客户端配置
│   │   ├── auth.ts            # 认证相关 API
│   │   ├── menu.ts            # 菜单相关 API
│   │   └── types.ts            # API 类型定义
│   ├── components/            # 可复用组件
│   │   ├── common/           # 通用组件
│   │   └── menu/             # 菜单相关组件
│   ├── pages/                # 页面组件
│   │   └── menu/             # 菜单管理页面
│   ├── hooks/                # 自定义 Hooks
│   │   ├── useMenu.ts        # 菜单相关 Hooks
│   │   └── useAuth.ts        # 认证相关 Hooks
│   ├── store/                # 状态管理
│   │   ├── menu.ts           # 菜单状态
│   │   └── auth.ts           # 认证状态
│   ├── utils/                # 工具函数
│   │   ├── request.ts        # 请求封装
│   │   ├── errorHandler.ts   # 错误处理
│   │   └── tree.ts           # 树形结构工具
│   ├── types/                # TypeScript 类型定义
│   │   └── menu.ts           # 菜单类型
│   ├── constants/            # 常量定义
│   │   └── menu.ts           # 菜单常量
│   └── App.tsx               # 根组件
├── public/                   # 静态资源
├── package.json
└── tsconfig.json
```

---

## 技术栈

### 核心框架

- **React 19.2+** - UI构建库
- **TypeScript** - 类型安全
- **Vite** - 构建工具

### UI 组件库

- **Ant Design 5.29+** - UI组件库
- **@ant-design/pro-components** - 企业级高级组件（ProTable、ProForm、ProLayout 等）

### 路由管理

- **React Router 7.9+** - 路由管理

### HTTP 客户端

- **Axios** - HTTP请求库

### 表单处理

- **React Hook Form** - 表单处理

### 日期处理

- **Day.js** - 日期处理库

### 状态管理（可选）

- **@tanstack/react-query** - 服务端状态管理（推荐用于 API 数据管理）

---

## 项目初始化

### 1. 使用 Vite 创建项目

```bash
# 创建 React + TypeScript 项目
npm create vite@latest frontend -- --template react-ts

# 或使用 yarn
yarn create vite frontend --template react-ts

# 进入项目目录
cd frontend
```

### 2. 安装依赖

```bash
# 安装核心依赖
npm install react@^19.2.0 react-dom@^19.2.0
npm install -D typescript @types/react @types/react-dom

# 安装 UI 组件库
npm install antd@^5.29.0 @ant-design/pro-components@latest

# 安装路由
npm install react-router@^7.9.0

# 安装 HTTP 客户端
npm install axios

# 安装表单处理
npm install react-hook-form

# 安装日期处理
npm install dayjs

# 安装状态管理（可选）
npm install @tanstack/react-query

# 安装开发依赖
npm install -D vite @vitejs/plugin-react
```

### 3. package.json 示例

```json
{
  "name": "menu-management-frontend",
  "version": "1.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc && vite build",
    "preview": "vite preview",
    "lint": "eslint . --ext ts,tsx --report-unused-disable-directives --max-warnings 0"
  },
  "dependencies": {
    "react": "^19.2.0",
    "react-dom": "^19.2.0",
    "react-router": "^7.9.0",
    "antd": "^5.29.0",
    "@ant-design/pro-components": "^2.7.0",
    "axios": "^1.6.0",
    "react-hook-form": "^7.49.0",
    "dayjs": "^1.11.10",
    "@tanstack/react-query": "^5.17.0"
  },
  "devDependencies": {
    "@types/react": "^18.2.0",
    "@types/react-dom": "^18.2.0",
    "@typescript-eslint/eslint-plugin": "^6.0.0",
    "@typescript-eslint/parser": "^6.0.0",
    "@vitejs/plugin-react": "^4.2.0",
    "eslint": "^8.57.0",
    "eslint-plugin-react-hooks": "^4.6.0",
    "eslint-plugin-react-refresh": "^0.4.5",
    "typescript": "^5.2.2",
    "vite": "^5.0.0"
  }
}
```

### 4. Vite 配置

```typescript
// vite.config.ts
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import path from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
      },
    },
  },
});
```

### 5. TypeScript 配置

```json
// tsconfig.json
{
  "compilerOptions": {
    "target": "ES2020",
    "useDefineForClassFields": true,
    "lib": ["ES2020", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "skipLibCheck": true,
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "resolveJsonModule": true,
    "isolatedModules": true,
    "noEmit": true,
    "jsx": "react-jsx",
    "strict": true,
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "noFallthroughCasesInSwitch": true,
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"]
    }
  },
  "include": ["src"],
  "references": [{ "path": "./tsconfig.node.json" }]
}
```

---

## API 集成

### 1. HTTP 客户端配置

#### 使用 Axios 示例

```typescript
// src/api/client.ts
import axios, { AxiosInstance, AxiosError, InternalAxiosRequestConfig } from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

// 创建 axios 实例
export const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 请求拦截器 - 添加 Token
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('access_token');
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

// 响应拦截器 - 统一错误处理
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    if (error.response) {
      const { status, data } = error.response;
      
      switch (status) {
        case 401:
          // Token 过期，跳转到登录页
          localStorage.removeItem('access_token');
          window.location.href = '/login';
          break;
        case 403:
          // 权限不足
          console.error('权限不足');
          break;
        case 404:
          // 资源不存在
          console.error('资源不存在');
          break;
        case 409:
          // 资源冲突
          console.error('资源冲突，请检查数据');
          break;
        default:
          console.error('请求失败:', data);
      }
    } else if (error.request) {
      console.error('网络错误，请检查网络连接');
    } else {
      console.error('请求配置错误:', error.message);
    }
    
    return Promise.reject(error);
  }
);
```

### 2. API 服务封装

#### 菜单 API 服务

```typescript
// src/api/menu.ts
import { apiClient } from './client';
import type { MenuDto, CreateMenuDto, UpdateMenuDto, PagedResultDto } from '../types/menu';

/**
 * 获取树形菜单列表
 */
export const getMenuTree = async (): Promise<MenuDto[]> => {
  const response = await apiClient.get<MenuDto[]>('/api/menus/tree');
  return response.data;
};

/**
 * 获取分页菜单列表
 */
export const getMenuList = async (
  params?: {
    skipCount?: number;
    maxResultCount?: number;
    sorting?: string;
  }
): Promise<PagedResultDto<MenuDto>> => {
  const response = await apiClient.get<PagedResultDto<MenuDto>>('/api/menus', { params });
  return response.data;
};

/**
 * 根据ID获取菜单
 */
export const getMenuById = async (id: string): Promise<MenuDto> => {
  const response = await apiClient.get<MenuDto>(`/api/menus/${id}`);
  return response.data;
};

/**
 * 创建菜单
 */
export const createMenu = async (data: CreateMenuDto): Promise<MenuDto> => {
  const response = await apiClient.post<MenuDto>('/api/menus', data);
  return response.data;
};

/**
 * 更新菜单
 */
export const updateMenu = async (id: string, data: UpdateMenuDto): Promise<MenuDto> => {
  const response = await apiClient.put<MenuDto>(`/api/menus/${id}`, data);
  return response.data;
};

/**
 * 删除菜单
 */
export const deleteMenu = async (id: string): Promise<void> => {
  await apiClient.delete(`/api/menus/${id}`);
};

/**
 * 启用/禁用菜单
 */
export const setMenuStatus = async (id: string, enabled: boolean): Promise<void> => {
  await apiClient.put(`/api/menus/${id}/status`, null, {
    params: { enabled },
  });
};

/**
 * 根据角色ID获取菜单
 */
export const getMenusByRoleId = async (roleId: string): Promise<MenuDto[]> => {
  const response = await apiClient.get<MenuDto[]>(`/api/menus/role/${roleId}`);
  return response.data;
};

/**
 * 根据用户ID获取菜单
 */
export const getMenusByUserId = async (userId: string): Promise<MenuDto[]> => {
  const response = await apiClient.get<MenuDto[]>(`/api/menus/user/${userId}`);
  return response.data;
};

/**
 * 根据组织ID获取菜单
 */
export const getMenusByOrganizationId = async (organizationId: string): Promise<MenuDto[]> => {
  const response = await apiClient.get<MenuDto[]>(`/api/menus/organization/${organizationId}`);
  return response.data;
};

/**
 * 分配菜单给角色
 */
export const assignMenusToRole = async (roleId: string, menuIds: string[]): Promise<void> => {
  await apiClient.post(`/api/menus/role/${roleId}/assign`, menuIds);
};

/**
 * 分配菜单给组织
 */
export const assignMenusToOrganization = async (
  organizationId: string,
  menuIds: string[]
): Promise<void> => {
  await apiClient.post(`/api/menus/organization/${organizationId}/assign`, menuIds);
};
```

### 3. 使用 TanStack Query（推荐）

> **注意**: TanStack Query（原 React Query）用于管理服务端状态，提供缓存、自动重新获取等功能。

```typescript
// src/hooks/useMenu.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import * as menuApi from '../api/menu';
import type { MenuDto, CreateMenuDto, UpdateMenuDto } from '../types/menu';

// Query Keys
export const menuKeys = {
  all: ['menus'] as const,
  lists: () => [...menuKeys.all, 'list'] as const,
  list: (params?: any) => [...menuKeys.lists(), params] as const,
  details: () => [...menuKeys.all, 'detail'] as const,
  detail: (id: string) => [...menuKeys.details(), id] as const,
  tree: () => [...menuKeys.all, 'tree'] as const,
  byRole: (roleId: string) => [...menuKeys.all, 'role', roleId] as const,
  byUser: (userId: string) => [...menuKeys.all, 'user', userId] as const,
  byOrganization: (orgId: string) => [...menuKeys.all, 'organization', orgId] as const,
};

/**
 * 获取菜单树
 */
export const useMenuTree = () => {
  return useQuery({
    queryKey: menuKeys.tree(),
    queryFn: menuApi.getMenuTree,
    staleTime: 5 * 60 * 1000, // 5分钟缓存
  });
};

/**
 * 获取菜单列表（分页）
 */
export const useMenuList = (params?: {
  skipCount?: number;
  maxResultCount?: number;
  sorting?: string;
}) => {
  return useQuery({
    queryKey: menuKeys.list(params),
    queryFn: () => menuApi.getMenuList(params),
  });
};

/**
 * 获取菜单详情
 */
export const useMenu = (id: string) => {
  return useQuery({
    queryKey: menuKeys.detail(id),
    queryFn: () => menuApi.getMenuById(id),
    enabled: !!id,
  });
};

/**
 * 创建菜单
 */
export const useCreateMenu = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: menuApi.createMenu,
    onSuccess: () => {
      // 使相关查询失效，触发重新获取
      queryClient.invalidateQueries({ queryKey: menuKeys.all });
    },
  });
};

/**
 * 更新菜单
 */
export const useUpdateMenu = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateMenuDto }) =>
      menuApi.updateMenu(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: menuKeys.detail(variables.id) });
      queryClient.invalidateQueries({ queryKey: menuKeys.tree() });
    },
  });
};

/**
 * 删除菜单
 */
export const useDeleteMenu = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: menuApi.deleteMenu,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuKeys.all });
    },
  });
};

/**
 * 设置菜单状态
 */
export const useSetMenuStatus = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, enabled }: { id: string; enabled: boolean }) =>
      menuApi.setMenuStatus(id, enabled),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: menuKeys.all });
    },
  });
};
```

---

## 类型定义

### 菜单类型定义

```typescript
// src/types/menu.ts

/**
 * 菜单类型枚举
 */
export enum MenuType {
  Directory = 1, // 目录
  Menu = 2,     // 菜单
  Button = 3,    // 按钮
}

/**
 * 菜单状态枚举
 */
export enum MenuStatus {
  Disabled = 0, // 禁用
  Enabled = 1,  // 启用
}

/**
 * 菜单 DTO
 */
export interface MenuDto {
  id: string;
  name: string;
  code: string;
  parentId: string | null;
  parentName: string | null;
  type: MenuType;
  path: string | null;
  component: string | null;
  icon: string | null;
  sort: number;
  status: MenuStatus;
  permission: string | null;
  isHidden: boolean;
  isCache: boolean;
  isExternal: boolean;
  externalUrl: string | null;
  remark: string | null;
  children: MenuDto[];
  creationTime: string;
  lastModificationTime: string | null;
  creatorId: string | null;
  lastModifierId: string | null;
}

/**
 * 创建菜单 DTO
 */
export interface CreateMenuDto {
  name: string;
  code: string;
  parentId: string | null;
  type: MenuType;
  path?: string | null;
  component?: string | null;
  icon?: string | null;
  sort: number;
  status?: MenuStatus;
  permission?: string | null;
  isHidden?: boolean;
  isCache?: boolean;
  isExternal?: boolean;
  externalUrl?: string | null;
  remark?: string | null;
}

/**
 * 更新菜单 DTO
 */
export interface UpdateMenuDto {
  name: string;
  code: string;
  parentId: string | null;
  type: MenuType;
  path?: string | null;
  component?: string | null;
  icon?: string | null;
  sort: number;
  status: MenuStatus;
  permission?: string | null;
  isHidden: boolean;
  isCache: boolean;
  isExternal: boolean;
  externalUrl?: string | null;
  remark?: string | null;
}

/**
 * 分页结果 DTO
 */
export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
}

/**
 * 分页请求参数
 */
export interface PagedAndSortedResultRequestDto {
  skipCount?: number;
  maxResultCount?: number;
  sorting?: string;
}
```

---

## 状态管理

### 使用 React Context + useReducer（推荐）

对于简单的客户端状态，可以使用 React 内置的状态管理：

```typescript
// src/store/menu.tsx
import { createContext, useContext, useReducer, ReactNode } from 'react';
import type { MenuDto } from '../types/menu';

interface MenuState {
  selectedMenu: MenuDto | null;
  expandedKeys: string[];
}

type MenuAction =
  | { type: 'SET_SELECTED_MENU'; payload: MenuDto | null }
  | { type: 'SET_EXPANDED_KEYS'; payload: string[] };

const initialState: MenuState = {
  selectedMenu: null,
  expandedKeys: [],
};

function menuReducer(state: MenuState, action: MenuAction): MenuState {
  switch (action.type) {
    case 'SET_SELECTED_MENU':
      return { ...state, selectedMenu: action.payload };
    case 'SET_EXPANDED_KEYS':
      return { ...state, expandedKeys: action.payload };
    default:
      return state;
  }
}

interface MenuContextValue {
  state: MenuState;
  dispatch: React.Dispatch<MenuAction>;
}

const MenuContext = createContext<MenuContextValue | undefined>(undefined);

export function MenuProvider({ children }: { children: ReactNode }) {
  const [state, dispatch] = useReducer(menuReducer, initialState);
  return (
    <MenuContext.Provider value={{ state, dispatch }}>
      {children}
    </MenuContext.Provider>
  );
}

export function useMenuStore() {
  const context = useContext(MenuContext);
  if (!context) {
    throw new Error('useMenuStore must be used within MenuProvider');
  }
  return context;
}
```

### 使用 TanStack Query 管理服务端状态

服务端数据（如菜单列表、用户信息等）建议使用 TanStack Query 管理，详见 [API 集成](#api-集成) 部分。

---

## 错误处理

### 统一错误处理工具

```typescript
// src/utils/errorHandler.ts
import { AxiosError } from 'axios';
import { message } from 'antd';

export interface ApiError {
  status: number;
  message: string;
  errors?: Record<string, string[]>;
}

export const handleApiError = (error: unknown): ApiError => {
  if (error instanceof AxiosError) {
    const { response } = error;
    
    if (response) {
      const apiError: ApiError = {
        status: response.status,
        message: (response.data as any)?.title || response.statusText,
        errors: (response.data as any)?.errors,
      };
      
      // 使用 Ant Design message 显示错误消息
      message.error(apiError.message);
      
      return apiError;
    }
  }
  
  const defaultError: ApiError = {
    status: 500,
    message: '未知错误',
  };
  
  message.error(defaultError.message);
  return defaultError;
};
```

---

## 性能优化

### 1. 菜单树缓存

```typescript
// 使用 TanStack Query 的缓存机制
const { data: menuTree } = useMenuTree(); // 自动缓存5分钟

// 手动缓存到 localStorage（可选，TanStack Query 已提供缓存）
export const cacheMenuTree = (tree: MenuDto[]) => {
  localStorage.setItem('menu_tree_cache', JSON.stringify(tree));
  localStorage.setItem('menu_tree_cache_time', Date.now().toString());
};

export const getCachedMenuTree = (): MenuDto[] | null => {
  const cached = localStorage.getItem('menu_tree_cache');
  const cacheTime = localStorage.getItem('menu_tree_cache_time');
  
  if (cached && cacheTime) {
    const age = Date.now() - parseInt(cacheTime);
    if (age < 5 * 60 * 1000) { // 5分钟内有效
      return JSON.parse(cached);
    }
  }
  
  return null;
};
```

### 2. 虚拟滚动（大量数据）

```typescript
// 使用 react-window 或 react-virtualized
import { FixedSizeList } from 'react-window';

// 对于长列表，使用虚拟滚动
<FixedSizeList
  height={600}
  itemCount={menus.length}
  itemSize={50}
  width="100%"
>
  {({ index, style }) => (
    <div style={style}>
      {menus[index].name}
    </div>
  )}
</FixedSizeList>
```

### 3. 懒加载子菜单

```typescript
// 只在展开时加载子菜单
const useLazyMenuChildren = (menuId: string, expanded: boolean) => {
  return useQuery({
    queryKey: ['menu-children', menuId],
    queryFn: () => menuApi.getMenuChildren(menuId),
    enabled: expanded, // 只在展开时查询
  });
};
```

### 4. 使用 Ant Design Pro Components

Ant Design Pro Components 提供了企业级的高级组件，可以大幅提升开发效率：

```typescript
// src/pages/menu/index.tsx
import { ProTable } from '@ant-design/pro-components';
import type { ProColumns } from '@ant-design/pro-components';
import { useMenuList, useDeleteMenu } from '@/hooks/useMenu';
import type { MenuDto } from '@/types/menu';
import { Button, Space, Popconfirm } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';

const MenuListPage = () => {
  const { data, isLoading, refetch } = useMenuList();
  const deleteMenu = useDeleteMenu();

  const columns: ProColumns<MenuDto>[] = [
    {
      title: '菜单名称',
      dataIndex: 'name',
      key: 'name',
    },
    {
      title: '菜单编码',
      dataIndex: 'code',
      key: 'code',
    },
    {
      title: '类型',
      dataIndex: 'type',
      key: 'type',
      valueEnum: {
        1: { text: '目录' },
        2: { text: '菜单' },
        3: { text: '按钮' },
      },
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      valueEnum: {
        0: { text: '禁用', status: 'Error' },
        1: { text: '启用', status: 'Success' },
      },
    },
    {
      title: '操作',
      key: 'action',
      render: (_, record) => (
        <Space>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => handleEdit(record)}
          >
            编辑
          </Button>
          <Popconfirm
            title="确定要删除吗？"
            onConfirm={() => handleDelete(record.id)}
          >
            <Button
              type="link"
              danger
              icon={<DeleteOutlined />}
            >
              删除
            </Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  const handleDelete = async (id: string) => {
    await deleteMenu.mutateAsync(id);
    message.success('删除成功');
    refetch();
  };

  return (
    <ProTable<MenuDto>
      columns={columns}
      dataSource={data?.items}
      loading={isLoading}
      rowKey="id"
      search={{
        labelWidth: 'auto',
      }}
      pagination={{
        total: data?.totalCount,
        pageSize: 10,
      }}
      toolBarRender={() => [
        <Button key="add" type="primary">
          新建菜单
        </Button>,
      ]}
    />
  );
};
```

---

## 安全性

### 1. Token 管理

```typescript
// src/utils/auth.ts
const TOKEN_KEY = 'access_token';
const REFRESH_TOKEN_KEY = 'refresh_token';

export const getToken = (): string | null => {
  return localStorage.getItem(TOKEN_KEY);
};

export const setToken = (token: string): void => {
  localStorage.setItem(TOKEN_KEY, token);
};

export const removeToken = (): void => {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
};

// Token 自动刷新
export const refreshToken = async (): Promise<string | null> => {
  try {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    if (!refreshToken) return null;
    
    const response = await apiClient.post('/connect/token', {
      grant_type: 'refresh_token',
      refresh_token: refreshToken,
    });
    
    const { access_token } = response.data;
    setToken(access_token);
    return access_token;
  } catch (error) {
    removeToken();
    return null;
  }
};
```

### 2. XSS 防护

```typescript
// 使用 DOMPurify 清理用户输入
import DOMPurify from 'dompurify';

export const sanitizeHtml = (html: string): string => {
  return DOMPurify.sanitize(html);
};
```

### 3. CSRF 防护

```typescript
// 从 Cookie 中读取 CSRF Token（如果后端支持）
const getCsrfToken = (): string | null => {
  const cookies = document.cookie.split(';');
  const csrfCookie = cookies.find(c => c.trim().startsWith('XSRF-TOKEN='));
  return csrfCookie ? csrfCookie.split('=')[1] : null;
};
```

### 4. 日期处理（Day.js）

```typescript
// src/utils/date.ts
import dayjs from 'dayjs';
import 'dayjs/locale/zh-cn'; // 中文 locale
import relativeTime from 'dayjs/plugin/relativeTime';
import customParseFormat from 'dayjs/plugin/customParseFormat';

dayjs.extend(relativeTime);
dayjs.extend(customParseFormat);
dayjs.locale('zh-cn');

// 格式化日期
export const formatDate = (date: string | Date, format = 'YYYY-MM-DD HH:mm:ss'): string => {
  return dayjs(date).format(format);
};

// 相对时间
export const formatRelativeTime = (date: string | Date): string => {
  return dayjs(date).fromNow();
};

// 在组件中使用
import { formatDate } from '@/utils/date';

const MenuItem = ({ menu }: { menu: MenuDto }) => {
  return (
    <div>
      <span>创建时间: {formatDate(menu.creationTime)}</span>
      <span>更新时间: {formatDate(menu.lastModificationTime || menu.creationTime)}</span>
    </div>
  );
};
```

---

## 代码规范

### 1. ESLint 配置

```json
// .eslintrc.json
{
  "extends": [
    "eslint:recommended",
    "plugin:@typescript-eslint/recommended",
    "plugin:react/recommended",
    "plugin:react-hooks/recommended"
  ],
  "rules": {
    "react/react-in-jsx-scope": "off",
    "@typescript-eslint/explicit-function-return-type": "warn",
    "@typescript-eslint/no-unused-vars": "error"
  }
}
```

### 2. Prettier 配置

```json
// .prettierrc
{
  "semi": true,
  "trailingComma": "es5",
  "singleQuote": true,
  "printWidth": 100,
  "tabWidth": 2
}
```

### 3. 命名规范

- **组件**: PascalCase (`MenuTree.tsx`)
- **Hook**: camelCase with `use` prefix (`useMenu.ts`)
- **工具函数**: camelCase (`formatDate.ts`)
- **常量**: UPPER_SNAKE_CASE (`API_BASE_URL`)
- **类型/接口**: PascalCase (`MenuDto`)

---

## 测试策略

### 1. 单元测试（Vitest）

```typescript
// src/utils/__tests__/tree.test.ts
import { describe, it, expect } from 'vitest';
import { buildMenuTree } from '../tree';

describe('buildMenuTree', () => {
  it('should build tree structure correctly', () => {
    const menus = [
      { id: '1', parentId: null, name: 'Root' },
      { id: '2', parentId: '1', name: 'Child' },
    ];
    
    const tree = buildMenuTree(menus);
    expect(tree).toHaveLength(1);
    expect(tree[0].children).toHaveLength(1);
  });
});
```

### 2. 组件测试（React Testing Library）

```typescript
// src/components/__tests__/MenuTree.test.tsx
import { render, screen } from '@testing-library/react';
import { MenuTree } from '../MenuTree';

describe('MenuTree', () => {
  it('should render menu tree', () => {
    const menus = [
      { id: '1', name: 'Root', children: [] },
    ];
    
    render(<MenuTree menus={menus} />);
    expect(screen.getByText('Root')).toBeInTheDocument();
  });
});
```

### 3. E2E 测试（Playwright）

```typescript
// e2e/menu.spec.ts
import { test, expect } from '@playwright/test';

test('should create menu', async ({ page }) => {
  await page.goto('/menu');
  await page.click('button:has-text("新建")');
  await page.fill('input[name="name"]', '测试菜单');
  await page.fill('input[name="code"]', 'test-menu');
  await page.click('button:has-text("保存")');
  
  await expect(page.locator('text=测试菜单')).toBeVisible();
});
```

---

## 常见问题

### 1. 菜单树渲染性能问题

**问题**: 菜单树数据量大时渲染卡顿

**解决方案**:
- 使用虚拟滚动
- 懒加载子节点
- 使用 `React.memo` 优化组件
- 使用 `useMemo` 缓存计算结果

### 2. Token 过期处理

**问题**: Token 过期后需要重新登录

**解决方案**:
- 实现 Token 自动刷新机制
- 在请求拦截器中检查 Token 过期时间
- 过期前自动刷新

### 3. 菜单权限控制

**问题**: 根据用户权限动态显示菜单

**解决方案**:
```typescript
// 使用用户菜单 API
const { data: userMenus } = useQuery({
  queryKey: ['user-menus', userId],
  queryFn: () => menuApi.getMenusByUserId(userId),
});

// 根据权限过滤菜单
const filteredMenus = useMemo(() => {
  return filterMenusByPermission(userMenus, userPermissions);
}, [userMenus, userPermissions]);
```

### 5. React Router 7.x 路由配置

**问题**: 如何配置 React Router 7.x

**解决方案**:
```typescript
// src/router/index.tsx
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { ProLayout } from '@ant-design/pro-components';
import MenuListPage from '@/pages/menu';
import MenuDetailPage from '@/pages/menu/detail';

const router = createBrowserRouter([
  {
    path: '/',
    element: <ProLayout />,
    children: [
      {
        path: 'menu',
        element: <MenuListPage />,
      },
      {
        path: 'menu/:id',
        element: <MenuDetailPage />,
      },
    ],
  },
]);

export default function AppRouter() {
  return <RouterProvider router={router} />;
}
```

### 6. React Hook Form 与 Ant Design 集成

**问题**: 如何在 Ant Design 表单中使用 React Hook Form

**解决方案**:
```typescript
// src/components/MenuForm.tsx
import { useForm, Controller } from 'react-hook-form';
import { Form, Input, Select, Button } from 'antd';
import type { CreateMenuDto } from '@/types/menu';
import { MenuType, MenuStatus } from '@/types/menu';

interface MenuFormProps {
  onSubmit: (data: CreateMenuDto) => void;
  initialValues?: CreateMenuDto;
}

export const MenuForm = ({ onSubmit, initialValues }: MenuFormProps) => {
  const { control, handleSubmit, formState: { errors } } = useForm<CreateMenuDto>({
    defaultValues: initialValues,
  });

  return (
    <Form onFinish={handleSubmit(onSubmit)} layout="vertical">
      <Controller
        name="name"
        control={control}
        rules={{ required: '请输入菜单名称' }}
        render={({ field }) => (
          <Form.Item
            label="菜单名称"
            validateStatus={errors.name ? 'error' : ''}
            help={errors.name?.message}
          >
            <Input {...field} />
          </Form.Item>
        )}
      />
      <Controller
        name="type"
        control={control}
        rules={{ required: '请选择菜单类型' }}
        render={({ field }) => (
          <Form.Item
            label="菜单类型"
            validateStatus={errors.type ? 'error' : ''}
            help={errors.type?.message}
          >
            <Select {...field}>
              <Select.Option value={MenuType.Directory}>目录</Select.Option>
              <Select.Option value={MenuType.Menu}>菜单</Select.Option>
              <Select.Option value={MenuType.Button}>按钮</Select.Option>
            </Select>
          </Form.Item>
        )}
      />
      <Form.Item>
        <Button type="primary" htmlType="submit">
          提交
        </Button>
      </Form.Item>
    </Form>
  );
};
```

### 4. 菜单拖拽排序

**问题**: 实现菜单拖拽排序功能

**解决方案**:
- 使用 `react-beautiful-dnd` 或 `@dnd-kit/core`
- 拖拽后调用更新 API 更新 `sort` 字段

---

## 参考资源

### 官方文档

- [后端 API 文档](../API_DOCUMENTATION.md)
- [接口测试文件](../../MenuManagement.HttpApi.Host/MenuManagement.HttpApi.Host.http)
- [React 19 官方文档](https://react.dev/)
- [TypeScript 官方文档](https://www.typescriptlang.org/)
- [Vite 官方文档](https://vitejs.dev/)
- [Ant Design 5 官方文档](https://ant.design/)
- [Ant Design Pro Components 文档](https://procomponents.ant.design/)
- [React Router 7 官方文档](https://reactrouter.com/)
- [TanStack Query 文档](https://tanstack.com/query/latest)
- [Axios 文档](https://axios-http.com/)
- [React Hook Form 文档](https://react-hook-form.com/)
- [Day.js 文档](https://day.js.org/)

---

## 更新日志

### v1.1.0 (2024-01-01)
- 更新技术栈为 React 19.2+、Ant Design 5.29+、React Router 7.9+
- 添加 @ant-design/pro-components 使用示例
- 添加 Day.js 日期处理示例
- 添加 React Hook Form 与 Ant Design 集成示例
- 更新状态管理方案为 React Context + useReducer
- 优化代码示例和最佳实践

### v1.0.0 (2024-01-01)
- 初始版本
- 包含完整的 API 集成指南
- 类型定义和最佳实践

---

**文档维护者**: 前端团队  
**最后更新**: 2024-01-01
