using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Util;
using OGSAndroid;
using System.Text.RegularExpressions;

namespace OGSAndroid
{
    // ReSharper disable once InconsistentNaming
    public class SGFView : BoardView
    {
        public SGF<Move> Moves = new SGF<Move>();
        public int CurrentMove = 1;

        public SGFView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        
        }

        public void SetSGF(SGF<Move> s)
        {
            Moves = s;
            CurrentMove = Convert.ToInt32(s.Info.Handicap);
            Lines = Convert.ToInt32(s.Info.Size);
            ToStart();
        }

        public void PlaceUpTo(int max)
        {
            ClearBoard();
            for(int i=0;i<max;i++)
            {
                if (i > Moves.Tree.Nodes.Count-1)
                    return;

                var node = Moves.Tree.Nodes[i];

                while(node.Data.MType == Move.Type.Chat)
                {
                    i++;
                    if (i > Moves.Tree.Nodes.Count-1)
                    return;
                    node = Moves.Tree.Nodes[i];
                }

                PlaceStone(node.Data);

            }
        }

        public void ToEnd()
        {
            PlaceUpTo(Moves.Tree.Nodes.Count);
            CurrentMove = Moves.Tree.Nodes.Count - 1;
        }

        public void ToStart()
        {
            CurrentMove = Convert.ToInt32(Moves.Info.Handicap);
            PlaceUpTo(CurrentMove);
        }

        public void Next()
        {
            CurrentMove++;
            PlaceUpTo(CurrentMove);
        }

        public void Previous()
        {
            if(CurrentMove == 1) return;

            CurrentMove--;
            PlaceUpTo(CurrentMove);
        }
    }
}


