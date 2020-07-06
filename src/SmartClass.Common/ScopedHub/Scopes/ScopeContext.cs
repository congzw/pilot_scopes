using System;
using System.Collections.Generic;

namespace SmartClass.Common.ScopedHub.Scopes
{
    public class ScopeContext : IScopeKey , IHaveBags
    {
        public string ScopeId { get; set; }
        public IDictionary<string, object> Bags { get; set; } = BagsHelper.Create();

        public object GetItem(string key, object defaultValue = null)
        {
            return !this.Bags.ContainsKey(key) ? defaultValue : this.Bags[key];
        }

        public ScopeContext SetItem(string key, object value)
        {
            this.Bags[key] = value;
            return this;
        }
        
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
