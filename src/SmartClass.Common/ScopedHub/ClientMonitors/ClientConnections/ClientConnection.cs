using System;
using System.Collections.Generic;

namespace SmartClass.Common.ScopedHub.ClientMonitors.ClientConnections
{
    public interface IClientConnection : IScopeClientConnectionLocate
    {
        string ClientType { get; set; }
    }

    public class MyConnection : IClientConnection, IHaveBags
    {
        public MyConnection()
        {
            var now = GetDateNow();
            CreateAt = now;
            LastUpdateAt = now;
            ClientType = string.Empty;
            Bags = BagsHelper.Create();
        }
        public Guid Id { get; set; }
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string ClientType { get; set; }
        /// <summary>
        /// signalr链接的connectionId
        /// 值为string.empty标识掉线
        /// </summary>
        public string ConnectionId { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public IDictionary<string, object> Bags { get; set; }

        public void CopyTo(MyConnection conn)
        {
            MyModelHelper.TryCopyProperties(conn, this);
        }

        #region for extensions

        public static Func<DateTime> GetDateNow = () => DateHelper.Instance.GetDateNow();

        #endregion
    }
}
