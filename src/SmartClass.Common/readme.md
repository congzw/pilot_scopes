# ScopeHubs组件说明

对SignalR的连接管理组件进行改造和扩展，支持进一步Scope的逻辑划分
组件定位是通讯层，不应含有任何业务逻辑

## 概念解释

Scope: 逻辑划分的一个范围
IScopeHub: 支持逻辑范围划分的Hub接口
Client: 业务上定义的一个逻辑客户端，它可以跟Hub建立连接
Group: 组
ClientGroup： 客户端和组的关系
ClientConnection: 客户端跟Hub建立的连接实例，用于实时通讯（可以理解是websocket的一个连接，包含连接的状态数据）
ClientMethods: 代表客户端的方法，包含Method和MethodArgs参数

## LocateKeys

一些约定好的键的属性名称

- IScopeKey => ScopeId
- IClientKey => ClientId
- ISignalRConnectionKey => ConnectionId
- IGroupKey => Group

- IScopeClientLocate : IScopeKey, IClientKey
- IScopeGroupLocate : IScopeKey, IGroupKey
- IClientConnectionLocatee : IScopeKey, IClientKey, ISignalRConnectionKey, IScopeClientLocate
- IScopeClientGroupLocate : IScopeKey, IClientKey, IGroupKey, IScopeClientLocate, IScopeGroupLocate

### 关于IClientConnectionLocate的解释: 

- ConnectionId，页面刷新或重连会发生变化，它无法直接用来跟踪和维护业务上的状态
- ScopedId 业务上定义的范围
- ClientId 业务上定义的一个逻辑客户端的唯一标识
- ScopedId + ClientId => ConnectionId => ClientConnection(代表一个连接的状态数据)， 用它来记录Scope内的业务状态变化

## 组件划分

- ClientContext 包含连接到Scope的客户端的上下文信息，可从认证后的Claims中获得
- ScopeContext 包含当前Scope的上下文信息
- ClientMonitors 
- SignalREventBus 所有通讯的事件都经此类分发，用它统一控制和记录事件信息
- HubCons 约定的常量

## 关于ClientMethods

为了简化理解，移除了上一版本跟前端约定的两种调用模式：

- x ClientInvoke 客户端主动调用的方法，可用于双向通讯场景，例如：客户端 -> 中心 -> 客户端
- x ClientStub 客户端被动调用的方法，可用于单向通讯场景，例如：外部（Api或服务） -> 中心 -> 客户端

目前只保留一种的模式：ClientMethod， 目前支持Scope内的三种发送场景： 

scopeId（可根据上下文自动补齐） 整个范围内
scopeId + clientIds 范围内特定客户端
scopeId + groups 范围内特定组
