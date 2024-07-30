using Newtonsoft.Json;

namespace EkkoSoreeg.Web.DataSeed
{
    public static class SessionExtensions
    {
        private const string ExpirationPrefix = "Expiration:";

        public static void SetObjectAsJson(this ISession session, string key, object value, TimeSpan expiration)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            // Serialize the object to JSON
            var json = JsonConvert.SerializeObject(value, settings);
            session.SetString(key, json);

            // Store expiration time in a separate key
            var expirationKey = ExpirationPrefix + key;
            session.SetString(expirationKey, DateTime.UtcNow.Add(expiration).ToString("o"));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var expirationKey = ExpirationPrefix + key;
            var expirationString = session.GetString(expirationKey);

            if (DateTime.TryParse(expirationString, out var expirationTime) && DateTime.UtcNow > expirationTime)
            {
                // Item has expired
                session.Remove(key);
                session.Remove(expirationKey);
                return default;
            }

            var json = session.GetString(key);
            return json == null ? default : JsonConvert.DeserializeObject<T>(json);
        }
    }


}
