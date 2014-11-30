#region

using Android.Util;

#endregion

namespace UrlImageViewHelper
{
    public class SoftReferenceHashTable<TKey, TValue>
    {
        /*
		Dictionary<TKey, SoftReference<TValue>> table = new Dictionary<TKey, SoftReference<TValue>>();
		
		public TValue Put(TKey key, TValue value)
		{
			var newVal = new SoftReference<TValue>(value);

			if (table.ContainsKey(key))
				table[key] = newVal;
			else
				table.Add(key, newVal);

			return newVal.Get();
		}

		public TValue Get(TKey key)
		{
			if (!table.ContainsKey(key))
				return default(TValue);

			var val = table[key];

			if (val == null || val.Get() == null)
			{
				Android.Util.Log.Debug(UrlImageViewHelper.LOGTAG, key.ToString() + " Lost Reference");
				table.Remove(key);
				return default(TValue);
			}

			return val.Get();
		}
		 */

        private readonly LRUCache<TKey, TValue> cache = new LRUCache<TKey, TValue>(100);
        private readonly object cacheLock = new object();

        public TValue Put(TKey key, TValue value)
        {
            //var newVal = new SoftReference<TValue>(value);
            lock (cacheLock)
            {
                if (cache.ContainsKey(key))
                    cache[key] = value;
                else
                    cache.Add(key, value);

                return value;
            }
        }

        public TValue Get(TKey key)
        {
            lock (cacheLock)
            {
                if (!cache.ContainsKey(key))
                    return default(TValue);

                TValue val = cache[key];

                if (val == null)
                {
                    Log.Debug(UrlImageViewHelper.LOGTAG, key + " Lost Reference");
                    cache.Remove(key);
                    return default(TValue);
                }

                return val;
            }
        }

//		
//		 Hashtable<K, SoftReference<V>> mTable = new Hashtable<K, SoftReference<V>>();
//    
//    public V put(K key, V value) {
//        SoftReference<V> old = mTable.put(key, new SoftReference<V>(value));
//        if (old == null)
//            return null;
//        return old.get();
//    }
//    
//    public V get(K key) {
//        SoftReference<V> val = mTable.get(key);
//        if (val == null)
//            return null;
//        V ret = val.get();
//        if (ret == null)
//            mTable.remove(key);
//        return ret;
//    }
    }
}