#region

using System;

#endregion

namespace OGSAndroid.Game
{
    public class Move : Stone
    {

        public string ToXYString(bool @internal = true)
        {
            //OGS isn't 0 indexed so if not internal reduce by 1.
            var xx = @internal ? x : x - 1;
            var yy = @internal ? y : y - 1;

            return ((char) (97 + xx)) + ((char) (97 + yy)).ToString();
        }

        public static Move LettersToMove(string letter, Stone colour)
        {
            if (String.IsNullOrEmpty(letter))
                return null; //pass
            var x = letter[0]%32;
            var y = letter[1]%32;

            return new Move(colour, x, y);
        }

        public override string ToString()
        {
            return "Move: " + x + "-" + y;
        }
    }
}