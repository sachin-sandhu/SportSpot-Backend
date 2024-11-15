using System.Text.Json.Nodes;

namespace Integration_Test.Extensions
{
    public static class JsonObjectExtension
    {

        public static T Value<T>(this JsonNode jsonObject)
        {
            if (jsonObject == null)
                return (T)(object)null;
            if (typeof(T) == typeof(JsonArray))
                return (T)(object)jsonObject.AsArray();
            return jsonObject.AsValue().GetValue<T>();
        }

    }
}
