using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs
{
    public class SendContext
    {
        /// <summary>
        /// 如果不是从Hub连接发送的，则此字段为空
        /// </summary>
        public string SendConnectionId { get; set; }
        public SendFrom From { get; set; } = new SendFrom();
        public SendTo To { get; set; } = new SendTo();

        public static SendContext Create()
        {
            return new SendContext();
        }
    }

    public class SendFrom : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }
        public string ClientType { get; set; }
    }

    public class SendTo : IScopeKey
    {
        public string ScopeId { get; set; }
        public IList<string> ClientIds { get; set; } = new List<string>();
        public IList<string> Groups { get; set; } = new List<string>();
        public static SendTo CreateForScopeGroupAll(string scopeId)
        {
            var args = new SendTo();
            args.WithScopeId(scopeId);
            args.Groups.Add(HubConst.GroupName_All);
            return args;
        }

        public bool IsEmptyClientsAndGroups()
        {
            return ClientIds.Count == 0 && Groups.Count == 0;
        }
    }
}