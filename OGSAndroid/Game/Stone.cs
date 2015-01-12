#region

using System.Collections.Generic;
using System.Linq;
using System;

#endregion

namespace OGSAndroid.Game
{
    public class Stone //Can be used statically like an enum.
    {
        public static readonly Stone Black = new Stone(true);
        public static readonly Stone White = new Stone(false);
        public bool val;
        public int x;
        public int y;
        public bool Active;

        public Stone(bool _val, bool active = false)
        {
            val = _val;
            x = 0;
            y = 0;
            Active = active;
        }

        public Stone(bool _val, int _x, int _y, bool active = false)
        {
            val = _val;
            x = _x;
            y = _y;
            Active = active;
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

        public static bool InList(List<Stone> l, Stone s)
        {
            return l.Where(v => s != null)
                .Any(v => v
                    .Equals(s)
                          && v.x == s.x
                          && v.y == s.y);
        }

        public string ToXYString(bool @internal = true)
        {
            //OGS isn't 0 indexed so if not internal reduce by 1.
            var xx = @internal ? x : x - 1;
            var yy = @internal ? y : y - 1;

            return ((char) (97 + xx)) + ((char) (97 + yy)).ToString();
        }

        public static Stone LettersToMove(string letter, Stone colour)
        {
            //if (String.IsNullOrEmpty(letter))
               // return null; //pass
            var x = letter[0]%32;
            var y = letter[1]%32;

            return new Stone(colour, x, y);
        }

        public override string ToString()
        {
            return "Stone: " + x + "-" + y;
        }
    }
}