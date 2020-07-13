using System.Collections.Generic;
using SmartClass.Common.ScopeHubs;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;

namespace SmartClass.Web.Chats.AppServices
{
    public interface IChatAppService
    {
        void Chat(ChatArgs args);
    }

    public class ChatArgs
    {
        public string Message { get; set; }
        public SendFrom From { get; set; }
        public SendTo To { get; set; }
    }
}
