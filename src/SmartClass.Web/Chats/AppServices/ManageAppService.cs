using SmartClass.Common.ScopeHubs;

namespace SmartClass.Web.Chats.AppServices
{
    public interface IManageAppService
    {
        void Kick(KickArgs args);
    }

    public class KickArgs : IScopeClientLocate
    {
        public string ScopeId { get; set; }
        public string ClientId { get; set; }
    }
}
