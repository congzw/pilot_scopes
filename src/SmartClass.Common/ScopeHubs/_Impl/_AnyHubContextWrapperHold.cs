using System;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs._Impl
{
    public class _AnyHubContextWrapperHold : IHubContextWrapperHold
    {
        public _AnyHubContextWrapperHold(IHubContext<_AnyHub> hubContext)
        {
            if (hubContext == null) throw new ArgumentNullException(nameof(hubContext));
            Wrapper = hubContext.AsHubContextWrapper();
        }

        public HubContextWrapper Wrapper { get; set; }
    }
}