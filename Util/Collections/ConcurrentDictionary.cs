using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Collections
{
    public static class ConcurrentDictionary
    {
        /// <summary>
        /// Wraps the explicit implementation of ICollection.Remove, as per 
        /// http://blogs.msdn.com/b/pfxteam/archive/2011/04/02/10149222.aspx
        /// </summary>
        /// <returns><code>true</code> iff the key and unchanged value were found in the dictionary and removed.</returns>
        public static bool TryRemove<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            return dictionary.TryRemove(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Wraps the explicit implementation of ICollection.Remove, as per 
        /// http://blogs.msdn.com/b/pfxteam/archive/2011/04/02/10149222.aspx
        /// </summary>
        /// <returns><code>true</code> iff the key and unchanged value were found in the dictionary and removed.</returns>
        public static bool TryRemove<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dictionary, 
            KeyValuePair<TKey, TValue> keyValuePair)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(keyValuePair);
        }
    }
}
