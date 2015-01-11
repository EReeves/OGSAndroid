#region

using System;
using System.Linq;
using Android.Content;
using Android.Util;
using Android.Widget;

#endregion

namespace OGSAndroid.Game
{
    // ReSharper disable once InconsistentNaming
    public class SGFView : BoardView
    {
        private int currentMove = 1;
        public TextView MoveNumberText;
        public SGF<Move> Moves = new SGF<Move>();

        public SGFView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public int CurrentMove
        {
            get { return currentMove; }

            set
            {
                if (MoveNumberText == null)
                    throw new Exception("SGFView needs a MoveNumber TextView set on MoveNumberText");
                currentMove = value;
                MoveNumberText.Text = value.ToString();
            }
        }

        public void SetSGF(SGF<Move> s)
        {
            Moves = s;
            CurrentMove = Convert.ToInt32(s.Info.Handicap);
            Initialize(Convert.ToInt32(s.Info.Size));
            ToStart();
        }

        public void PlaceUpTo(int max)
        {
            ClearBoard();
            for (var i = 0; i < max; i++)
            {
                if (i > Moves.Tree.Nodes.Count - 1)
                {
                    CurrentMove = Moves.Tree.Nodes.Count - 1;
                    return;
                }

                var node = Moves.Tree.Nodes[i];

                PlaceStone(node.Data);
            }
        }

        public void ToEnd()
        {
            PlaceUpTo(Moves.Tree.Nodes.Count);
            CurrentMove = Moves.Tree.Nodes.Count;
        }

        public void ToStart()
        {
            CurrentMove = Convert.ToInt32(Moves.Info.Handicap);
            PlaceUpTo(CurrentMove);
        }

        public void Next()
        {
            if (currentMove > Moves.Tree.Nodes.Count() - 1)
                return;
            PlaceStone(Moves.Tree.Nodes[CurrentMove].Data);
            CurrentMove++;
        }

        public void Previous()
        {
            if (CurrentMove == 1) return;

            CurrentMove--;
            PlaceUpTo(CurrentMove);
        }
    }
}