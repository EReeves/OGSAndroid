﻿#region

using System.Collections.Generic;

#endregion

namespace Quobject.SocketIoClientDotNet.Collections.Concurrent
{
    public class ConcurrentQueue<T>
    {
        private readonly Queue<T> m_Queue;
        private readonly object m_SyncRoot = new object();

        public ConcurrentQueue()
        {
            m_Queue = new Queue<T>();
        }

        public ConcurrentQueue(int capacity)
        {
            m_Queue = new Queue<T>(capacity);
        }

        public ConcurrentQueue(IEnumerable<T> collection)
        {
            m_Queue = new Queue<T>(collection);
        }

        public int Count
        {
            get { return m_Queue.Count; }
        }

        public void Clear()
        {
            lock (m_SyncRoot)
            {
                m_Queue.Clear();
            }
        }

        public List<T> GetEnumerator()
        {
            lock (m_SyncRoot)
            {
                var result = new List<T>();
                foreach (var item in m_Queue)
                {
                    result.Add(item);
                }
                return result;
            }
        }

        public void Enqueue(T item)
        {
            lock (m_SyncRoot)
            {
                m_Queue.Enqueue(item);
            }
        }

        public bool TryDequeue(out T item)
        {
            lock (m_SyncRoot)
            {
                if (m_Queue.Count <= 0)
                {
                    item = default(T);
                    return false;
                }

                item = m_Queue.Dequeue();
                return true;
            }
        }
    }
}