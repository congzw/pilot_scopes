﻿using System;
using System.Collections.Generic;

namespace SmartClass.Common.ScopeHubs.ClientMonitors.Scopes
{
    public class ScopeContext : IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();

        #region for ut & di extensions

        public static ScopeContext GetScopeContext(string scopeId, bool createIfNotExist = true)
        {
            var scopeService = Resolve();
            return scopeService.GetScopeContext(scopeId, createIfNotExist);
        }

        private static readonly Lazy<IScopeRepository> LazyInstance = new Lazy<IScopeRepository>(() => new ScopeRepository());
        public static Func<IScopeRepository> Resolve { get; set; } = () => LazyInstance.Value;

        #endregion
    }
}