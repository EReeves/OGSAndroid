#region

using System.Collections.Generic;

#endregion

namespace OGSAndroid
{
    public class HiddenReference<T>
    {
        private static readonly Dictionary<int, T> table = new Dictionary<int, T>();
        private static int idgen;
        private readonly int id;

        public HiddenReference()
        {
            lock (table)
            {
                id = idgen ++;
            }
        }

        public T Value
        {
            get
            {
                lock (table)
                {
                    return table[id];
                }
            }
            set
            {
                lock (table)
                {
                    table[id] = value;
                }
            }
        }

        ~HiddenReference()
        {
            lock (table)
            {
                table.Remove(id);
            }
        }
    }
}