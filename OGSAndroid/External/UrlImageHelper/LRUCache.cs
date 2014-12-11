#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace OGSAndroid.External.UrlImageHelper
{
    public class LRUCache<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private object sync = new object();
        private readonly int capacity;
        private readonly Dictionary<TKey, TValue> data;
        private readonly ICollection<KeyValuePair<TKey, TValue>> dataAsCollection;
        private readonly IndexedLinkedList<TKey> lruList = new IndexedLinkedList<TKey>();

        public LRUCache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("capacity should always be bigger than 0");
            }

            data = new Dictionary<TKey, TValue>(capacity);
            dataAsCollection = data;
            this.capacity = capacity;
        }

        public void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                throw new ArgumentException("An attempt was made to insert a duplicate key in the cache.");
            }
        }

        public bool ContainsKey(TKey key)
        {
            return data.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return data.Keys; }
        }

        public bool Remove(TKey key)
        {
            var existed = data.Remove(key);
            lruList.Remove(key);
            return existed;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return data.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return data.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                var value = data[key];
                lruList.Remove(key);
                lruList.Add(key);
                return value;
            }
            set
            {
                data[key] = value;
                lruList.Remove(key);
                lruList.Add(key);

                if (data.Count > capacity)
                {
                    Remove(lruList.First);
                    lruList.RemoveFirst();
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            data.Clear();
            lruList.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dataAsCollection.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dataAsCollection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var removed = dataAsCollection.Remove(item);
            if (removed)
            {
                lruList.Remove(item.Key);
            }
            return removed;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dataAsCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) data).GetEnumerator();
        }

        public void ReclaimLRU(int itemsToReclaim)
        {
            while (--itemsToReclaim >= 0 && lruList.First != null)
                Remove(lruList.First);
        }
    }

    public class IndexedLinkedList<T>
    {
        private readonly LinkedList<T> data = new LinkedList<T>();
        private readonly Dictionary<T, LinkedListNode<T>> index = new Dictionary<T, LinkedListNode<T>>();

        public int Count
        {
            get { return data.Count; }
        }

        public T First
        {
            get { return data.First.Value; }
        }

        public void Add(T value)
        {
            index[value] = data.AddLast(value);
        }

        public void RemoveFirst()
        {
            index.Remove(data.First.Value);
            data.RemoveFirst();
        }

        public void Remove(T value)
        {
            LinkedListNode<T> node;
            if (index.TryGetValue(value, out node))
            {
                data.Remove(node);
                index.Remove(value);
            }
        }

        public void Clear()
        {
            data.Clear();
            index.Clear();
        }
    }
}