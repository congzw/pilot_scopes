using SmartClass.Common.Scopes;

// ReSharper disable once CheckNamespace
namespace SmartClass.Domains.TempDb
{
    public interface ITempDbService : IMyScoped
    {
        void ClearHblTempDb(ClearHblTempDbArgs args);
    }

    public class ClearHblTempDbArgs : IHaveScopeId
    {
        public string ScopeId { get; set; }
    }
}
