using System;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Http
{
    public static class HttpSessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            session.SetString(key, json);
        }

        public static TData GetObject<TData>(this ISession session, string key)
        {
            var json = session.GetString(key);

            if (string.IsNullOrEmpty(json))
                return default(TData);

            return JsonConvert.DeserializeObject<TData>(json);
        }
    }
}
