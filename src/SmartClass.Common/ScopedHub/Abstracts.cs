namespace SmartClass.Common.ScopedHub
{
    public interface IScopeKey
    {
        /// <summary>
        /// 连接端业务范围的唯一标识，例如某堂课的Id等场景。逻辑上划分的Hub范围
        /// </summary>
        string ScopeId { get; set; }
    }

    public interface IClientKey
    {
        /// <summary>
        /// 连接端Client的唯一标识，例如某设备的Id
        /// </summary>
        string ClientId { get; set; }
    }

    public interface IGroupKey
    {
        /// <summary>
        /// 组名
        /// </summary>
        string Group { get; set; }
    }

    public interface IScopeClientKey : IScopeKey, IClientKey
    {
    }
    
    public interface IConnectionKey
    {
        string ConnectionId { get; set; }
    }

    public interface IScopeClientConnectionKey : IScopeClientKey, IConnectionKey
    {
    }

    public interface IScopeGroupKey : IScopeKey, IGroupKey
    {
    }

    public interface IScopeClientGroupKey : IScopeKey, IClientKey, IGroupKey, IScopeClientKey, IScopeGroupKey
    {
    }
}
