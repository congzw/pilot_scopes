﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Scopes
{
    public interface IScopeManager
    {
        Task ResetScope(ResetScopeEvent theEvent);
        Task UpdateScope(UpdateScopeEvent theEvent);
    }

    #region events
    
    public class ResetScopeEvent : SignalREvent
    {
        public ResetScopeArgs Args { get; }

        public ResetScopeEvent(Hub raiseHub, ResetScopeArgs args) : base(raiseHub)
        {
            Args = args;
        }

        public ResetScopeEvent(HubContextWrapper context, ResetScopeArgs args) : base(context, new SendArgs())
        {
            Args = args;
        }
    }

    public class UpdateScopeEvent : SignalREvent
    {
        public UpdateScopeArgs Args { get; set; }

        public UpdateScopeEvent(Hub raiseHub, UpdateScopeArgs args) : base(raiseHub)
        {
            Args = args;
        }

        public UpdateScopeEvent(HubContextWrapper context, UpdateScopeArgs args) : base(context, new SendArgs())
        {
        }
    }

    public class ResetScopeArgs : IScopeKey, IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();
    }

    public class UpdateScopeArgs : IScopeKey, IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();
    }

    #endregion
}