#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace OGSAndroid
{
    public class Stone //Can be used statically like an enum.
    {
        public static readonly Stone Black = new Stone(true);
        public static readonly Stone White = new Stone(false);
        public int x;
        public int y;
        private readonly bool val;

        public Stone(bool _val)
        {
            val = _val;
        }

        public Stone(bool _val, int _x, int _y)
        {
            val = _val;
            x = _x;
            y = _y;
        }

        public static implicit operator bool(Stone s)
        {
            return s.val;
        }

        public override bool Equals(object obj)
        {
            return ((Stone) obj).val == val;
        }

        public bool Equals(Stone s)
        {
            return s.val == val;
        }

        public Move Move()
        {
            var mv = new Move(this);
            mv.x = x;
            mv.y = y;
            return mv;
        }

        public static bool InList(List<Stone> l, Stone s)
        {
            return l.Where(v => s != null)
                .Any(v => v
                    .Equals(s)
                          && v.x == s.x
                          && v.y == s.y);
        }
    }
}