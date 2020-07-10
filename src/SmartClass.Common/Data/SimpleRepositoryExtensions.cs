using System.Linq;

namespace SmartClass.Common.Data
{
    public static class SimpleRepositoryExtensions
    {
        public static void Add<T>(this ISimpleRepository simpleRepository, params T[] entities) where T : class
        {
            simpleRepository.Add(entities.ToArray());
        }

        public static void Update<T>(this ISimpleRepository simpleRepository, params T[] entities) where T : class
        {
            simpleRepository.Update(entities.ToArray());
        }

        public static void Delete<T>(this ISimpleRepository simpleRepository, params T[] entities) where T : class
        {
            simpleRepository.Delete(entities.ToArray());
        }
    }
}
