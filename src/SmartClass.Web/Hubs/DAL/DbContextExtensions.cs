using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

// ReSharper disable once CheckNamespace
namespace SmartClass.DAL
{
    public static class DbContextExtensions
    {
        public static PropertyBuilder<IDictionary<string, object>> HasBagsConversion(this PropertyBuilder<IDictionary<string, object>> propertyBuilder)
        {
            return propertyBuilder.HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<string, object>>(v));
        }

        public static PropertyBuilder<IList<string>> HasListConversion(this PropertyBuilder<IList<string>> propertyBuilder)
        {
            return propertyBuilder.HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<IList<string>>(v));
        }
    }
}
