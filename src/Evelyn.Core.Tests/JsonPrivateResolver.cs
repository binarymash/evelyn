namespace Evelyn.Core.Tests
{
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class JsonPrivateResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.Writable)
            {
                return property;
            }

            var propertyInfo = member as PropertyInfo;
            if (propertyInfo == null)
            {
                return property;
            }

            var hasPrivateSetter = propertyInfo.GetSetMethod(true) != null;
            property.Writable = hasPrivateSetter;

            return property;
        }
    }
}
