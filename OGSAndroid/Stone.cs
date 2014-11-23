using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OGSAndroid
{
    public class Stone //Can be used statically like an enum.
    {     
        public static readonly Stone Black = new Stone(true);
        public static readonly Stone White = new Stone(false);

        public int x;
        public int y;
        readonly bool val;

        public Stone(bool _val) { val = _val;}
        public Stone(bool _val, int _x, int _y) { val = _val; x = _x; y = _y; }
        public static implicit operator bool (Stone s) { return s.val; }      

        public override bool Equals(object obj)
        {
            return ((Stone)obj).val == this.val;
        }

        public bool Equals(Stone s)
        {
            return s.val == this.val;
        }

        public static bool InList(List<Stone> l  , Stone s)
        {
            foreach (var v in l)
            {
                if (s == null)
                    continue;
                if (v.Equals(s) && v.x == s.x && v.y == s.y)
                    return true;
            }

            return false;
        }
    }
}

