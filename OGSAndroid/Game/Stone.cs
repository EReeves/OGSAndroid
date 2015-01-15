#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace OGSAndroid.Game
{
    public class Stone //Can be used statically like an enum.
    {
        public static readonly Stone Black = new Stone(true);
        public static readonly Stone White = new Stone(false);
        public bool Active;
        public bool Val;
        public int X;
        public int Y;

        public Stone(bool val, bool active = false)
        {
            Val = val;
            X = 0;
            Y = 0;
            Active = active;
        }

        public Stone(bool val, int x, int y, bool active = false)
        {
            Val = val;
            X = x;
            Y = y;
            Active = active;
        }

        public static implicit operator bool(Stone s)
        {
            return s.Val;
        }
#pragma warning disable 659 //Bad design but we don't want a hash code.
        public override bool Equals(object obj)
        {
            return ((Stone) obj).Val == Val;
        }
#pragma warning restore 659

        public bool Equals(Stone s)
        {
            return s.Val == Val;
        }

        public static bool InList(List<Stone> l, Stone s)
        {
            return l.Where(v => s != null)
                .Any(v => v
                    .Equals(s)
                          && v.X == s.X
                          && v.Y == s.Y);
        }

        public string ToXYString(bool @internal = true)
        {
            //OGS isn't 0 indexed so if not internal reduce by 1.
            var xx = @internal ? X : X - 1;
            var yy = @internal ? Y : Y - 1;

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
            return "Stone: " + X + "-" + Y;
        }
    }
}