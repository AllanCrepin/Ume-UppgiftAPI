using System.Text.Json;

namespace UmeåUppgiftAPI.Helpers
{
    public static class SessionHelper
    {
        /// <summary>
        /// Sets an object in the session
        /// </summary>
        /// <typeparam name="T">Type of object to store</typeparam>
        /// <param name="session">The session object</param>
        /// <param name="key">Key to store the object under</param>
        /// <param name="value">The object to store</param>
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        
        /// <summary>
        /// Gets an object from the session
        /// </summary>
        /// <typeparam name="T">Type of object to retrieve</typeparam>
        /// <param name="session">The session object</param>
        /// <param name="key">Key the object is stored under</param>
        /// <returns>The retrieved object or default value if not found</returns>
        public static T? GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
        
        /// <summary>
        /// Removes an object from the session
        /// </summary>
        /// <param name="session">The session object</param>
        /// <param name="key">Key the object is stored under</param>
        public static void RemoveObject(this ISession session, string key)
        {
            session.Remove(key);
        }
    }
} 