#region

using System.Collections.Generic;

#endregion

namespace UrlImageViewHelper
{
    public class HashTable<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> hashset = new Dictionary<TKey, TValue>();
        private readonly object lockObj = new object();

        public void Put(TKey key, TValue value)
        {
            lock (lockObj)
            {
                if (hashset.ContainsKey(key))
                    hashset[key] = value;
                else
                    hashset.Add(key, value);
            }
        }

        public TValue Get(TKey key)
        {
            TValue result = default(TValue);

            lock (lockObj)
            {
                if (hashset.ContainsKey(key))
                    result = hashset[key];
            }

            return result;
        }

        public void Remove(TKey key)
        {
            lock (lockObj)
            {
                hashset.Remove(key);
            }
        }
    }
}