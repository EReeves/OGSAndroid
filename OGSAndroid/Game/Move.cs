﻿#region

using System;

#endregion

namespace OGSAndroid.Game
{
    public class Move : Stone
    {
        public enum Type
        {
            Place,
            Chat
        }

        public Type MType = Type.Place;

        public Move(bool _val) : base(_val)
        {
        }

        public Move(bool _val, int _x, int _y) : base(_val, _x, _y)
        {
        }

        public string Message { get; set; }
        public event Action Activation;

        public void Invoke()
        {
            Activation.Invoke();
        }

        public string ToXYString()
        {
            return ((char) (97 + x)) + ((char) (97 + y)).ToString();
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