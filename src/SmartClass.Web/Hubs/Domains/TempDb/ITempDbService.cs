using SmartClass.Common.ScopeHubs;

// ReSharper disable once CheckNamespace
namespace SmartClass.Domains.TempDb
{
    public interface ITempDbService : IMyScoped
    {
        void ClearHblTempDb(ClearHblTempDbArgs args);
    }

    public class ClearHblTempDbArgs : IScopeKey
    {
        public string ScopeId { get; set; }
    }
}
