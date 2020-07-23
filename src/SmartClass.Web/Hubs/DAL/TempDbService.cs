using System;
using System.Linq;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;
using SmartClass.Common.Scopes;
using SmartClass.Domains.TempDb;

// ReSharper disable once CheckNamespace
namespace SmartClass.DAL
{
    /// <summary>
    /// 清除临时数据服务
    /// </summary>
    public class TempDbService : ITempDbService
    {
        private readonly HblTempDbContext _hblTempDbContext;

        public TempDbService(HblTempDbContext hblTempDbContext)
        {
            _hblTempDbContext = hblTempDbContext;
        }

        //清空HblTempDb表
        public void ClearHblTempDb(ClearHblTempDbArgs args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (string.IsNullOrWhiteSpace(args.ScopeId))
            {
                return;
            }

            var changeTracker = _hblTempDbContext.ChangeTracker;
            //清除非MyConnection和ScopeClientGroup的数据
            //var theEntities = changeTracker.Entries().Select(c => c.Entity).OfType<IHaveScopeId>().Where(x => x.ScopeId == args.ScopeId).ToList();
            var theEntities = changeTracker.Entries()
                .Select(c => c.Entity)
                .Where(x => !(x is MyConnection) && !(x is ScopeClientGroup))
                .OfType<IHaveScopeId>()
                .Where(x => x.ScopeId == args.ScopeId).ToList();
            _hblTempDbContext.RemoveRange(theEntities);
            _hblTempDbContext.SaveChanges();
        }
    }
}
