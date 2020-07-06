﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SmartClass.Common.ScopedHub.Scopes
{
    public interface IScopeRepository
    {
        ScopeContext GetScopeContext(string scopeId, bool createIfNotExist);
        void SetScopeContext(ScopeContext scopeContext);
        void RemoveScopeContext(string scopeId);
        void ClearAll();
    }

    public class ScopeRepository : IScopeRepository
    {
        //default use memory dictionary impl, can also be replaced by other impl such as database source...
        public IDictionary<string, ScopeContext> Contexts { get; set; } = new ConcurrentDictionary<string, ScopeContext>(StringComparer.OrdinalIgnoreCase);

        public ScopeContext GetScopeContext(string scopeId, bool createIfNotExist)
        {
            if (!Contexts.ContainsKey(scopeId))
            {
                if (!createIfNotExist)
                {
                    return null;
                }

                var scopeContext = new ScopeContext { ScopeId = scopeId };
                Contexts[scopeId] = scopeContext;
            }
            return Contexts[scopeId];
        }

        public void SetScopeContext(ScopeContext scopeContext)
        {
            if (scopeContext == null)
            {
                throw new ArgumentNullException(nameof(scopeContext));
            }
            Contexts[scopeContext.ScopeId] = scopeContext;
        }

        public void RemoveScopeContext(string scopeId)
        {
            if (!Contexts.ContainsKey(scopeId))
            {
                return;
            }
            Contexts.Remove(scopeId);
        }

        public void ClearAll()
        {
            Contexts.Clear();
        }
    }
}
