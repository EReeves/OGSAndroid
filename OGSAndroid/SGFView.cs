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
        public MatchInfo Info = new MatchInfo();

        public SGFView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        
        }


        private static bool ContainsNotEscaped(char contains, string str)
        {
            for (var i = 0; i < str.Length; i++)
            {
                if (str[i] != contains) continue;
                if (i == 0) return true;
                if (str[i - 1] != '\\') return true;
            }
            return false;
        }

        private string[] GrabData(string line)
        {
            var splt = line.Split(new[]{ '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            var ncr = splt.Where(c => c != "\r");
            return ncr.Skip(1).ToArray(); //Skip definition, e.g. PB from PB[Rvzy]          
        }

        private void GrabHandicapStones(string line)
        {
            var strs = GrabData(line);
           
            //foreach (var s in strs)
                //Moves.Add(LettersToMove(s, Stone.Black));
        }

        public void PlaceToEnd()
        {
            foreach (var m in Moves.Tree)
            {
                switch (m.Data.MType)
                {
                    case Move.Type.Chat:
                        break;

                    case Move.Type.Place:
                        //PlaceStone(m);
                        break;
                }
            }
        }
    }
}


