#region

using Android.Util;

#endregion

namespace OGSAndroid.External.UrlImageHelper
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

                var val = cache[key];

                if (val == null)
                {
                    Log.Debug(UrlImageViewHelper.LOGTAG, key + " Lost Reference");
                    cache.Remove(key);
                    return default(TValue);
                }

                return val;
            }
        }
    }
}