namespace SmartClass.Common.ScopeHubs
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

    public interface ISignalRConnectionKey
    {
        string ConnectionId { get; set; }
    }

    public interface IGroupKey
    {
        /// <summary>
        /// 组名
        /// </summary>
        string Group { get; set; }
    }

    //scope client
    public interface IScopeClientLocate : IScopeKey, IClientKey
    {
    }

    //scope client connection
    public interface IClientConnectionLocate : IScopeKey, IClientKey, ISignalRConnectionKey, IScopeClientLocate
    {
    }

    //scope group
    public interface IScopeGroupLocate : IScopeKey, IGroupKey
    {
    }

    //scope client group
    public interface IScopeClientGroupLocate : IScopeKey, IClientKey, IGroupKey, IScopeClientLocate, IScopeGroupLocate
    {
    }

    #region locate model

    public class ClientConnectionLocate : IClientConnectionLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ConnectionId { get; set; }

        public static ClientConnectionLocate Create()
        {
            return new ClientConnectionLocate();
        }
    }
    public class ScopeClientLocate : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
    }
    public class ScopeGroupLocate : IScopeGroupLocate
    {
        public string ScopeId { get; set; }
        public string Group { get; set; }
    }
    public class ScopeClientGroupLocate : IScopeClientGroupLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string Group { get; set; }
    }
    
    #endregion
}
