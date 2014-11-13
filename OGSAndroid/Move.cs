using System;

namespace OGSAndroid
{
    public class Move : Stone
    {
        public Move(bool _val) : base(_val){}
        public Move(bool _val, int _x, int _y) : base(_val, _x, _y){}

        public event Action Activation;
        public Type MType = Move.Type.Place;
        public string Message {get;set;}

        public enum Type {
            Place,
            Chat,
            Malkovich
        }

        public void Invoke()
        {
            Activation.Invoke();
        }

        public static Move LettersToMove(string letter, Stone colour)
        {
            var x = letter[0] % 32;
            var y = letter[1] % 32;
            return new Move(colour, x, y);
        }

        public override string ToString()
        {
            return "Move: "+x+"-"+y;
        }
    }
}

