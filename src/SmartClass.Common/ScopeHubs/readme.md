# ScopeHubs组件说明

对SignalR的连接管理组件进行改造和扩展，支持进一步Scope的逻辑划分
组件定位是通讯层，不应含有任何业务逻辑

## 概念解释

Scope: 逻辑划分的一个范围
IScopeHub: 支持逻辑范围划分的Hub接口
Client: 代表客户端，可以跟Hub建立连接
Group: 组
ClientConnection: 客户端跟Hub建立的连接实例，用于实时通讯（可以理解是websocket的一个连接存储，代表一个连接的状态数据）

## LocateKeys

- IScopeKey => ScopeId
- IClientKey => ClientId
- ISignalRConnectionKey => SignalRConnectionId
- IGroupKey => Group

- IScopeClientLocate : IScopeKey, IClientKey
- IScopeGroupLocate : IScopeKey, IGroupKey
- IClientConnectionLocatee : IScopeKey, IClientKey, ISignalRConnectionKey, IScopeClientLocate
- IScopeClientGroupLocate : IScopeKey, IClientKey, IGroupKey, IScopeClientLocate, IScopeGroupLocate

### 关于IClientConnectionLocate的解释: 

- SignalRConnectionId，刷新会变化，无法直接用来跟踪和维护业务上的状态。
- ScopedId 业务上定义的范围（适用于多租户等场景）
- ClientId 业务上定义的一个客户端标识

ScopedId + ClientId => SignalRConnectionId => ClientConnection(代表一个连接的状态数据)

## 组件划分

- ScopeContext
- ClientMonitors
- EventBus
- ScopeHubConst

## 关于ClientMethods

移除了跟前端约定了两种调用模式：

- x ClientInvoke 客户端主动调用的方法，可用于双向通讯场景，例如：客户端 -> 中心 -> 客户端
- x ClientStub 客户端被动调用的方法，可用于单向通讯场景，例如：外部（Api或服务） -> 中心 -> 客户端

只保留一种统一的模式：ClientMethod


scopeId
scopeId + clientIds
scopeId + groups
