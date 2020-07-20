using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace SmartClass.Common.ScopeHubs
{
    public static class SignalrExtensions
    {
        public static HttpContext TryGetHttpContext(this Hub hub)
        {
            return hub?.Context?.GetHttpContext();
        }

        public static async Task AddToGroupsAsync(this IGroupManager groupManager, string connectionId, IEnumerable<string> groupNames, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var groupName in groupNames)
            {
                await groupManager.AddToGroupAsync(connectionId, groupName, cancellationToken);
            }
        }

        public static async Task RemoveFromGroupsAsync(this IGroupManager groupManager, string connectionId, IEnumerable<string> groupNames, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var groupName in groupNames)
            {
                await groupManager.RemoveFromGroupAsync(connectionId, groupName, cancellationToken);
            }
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return !list.Any();
        }
    }
}
