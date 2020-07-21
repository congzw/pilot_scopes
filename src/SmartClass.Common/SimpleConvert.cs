using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartClass.Common
{
    public interface ISafeConvert
    {
        T SafeConvertTo<T>(object value);
    }

    public static class SimpleConvert
    {
        public static T SafeConvertTo<T>(object value)
        {
            return Current == null ? SafeConvertHelper.DefaultConvertTo<T>(value) : Current.SafeConvertTo<T>(value);
        }

        #region for extensions


        private static Func<ISafeConvert> _safeConvertFunc;
        public static ISafeConvert Current => _safeConvertFunc?.Invoke();
        public static void Initialize(Func<ISafeConvert> safeConvertFunc)
        {
            _safeConvertFunc = safeConvertFunc ?? throw new ArgumentNullException(nameof(safeConvertFunc));
        }

        #endregion
    }

    internal class SafeConvertHelper
    {
        internal static T DefaultConvertTo<T>(object value)
        {
            if (value is T modelValue)
            {
                return modelValue;
            }
            //处理序列化
            if (value is JObject theJObject)
            {
                return theJObject.ToObject<T>();
            }
            var json = JsonConvert.SerializeObject(value);
            var argsT = JsonConvert.DeserializeObject<T>(json);
            return argsT;
        }
    }
}