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
        public List<Move> Moves = new List<Move>();
        public MatchInfo Info = new MatchInfo();

        public SGFView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public struct MatchInfo
        {
            public string Black;
            public string BlackRank;
            public string White;
            public string WhiteRank;
            public string Size;
            public string Komi;
            public string Result;
            public string Handicap;
            public string Ruleset;
            public string Date;
            public string Link;

            public string String()
            {
                return
                    "Black: " + Black + " (" + BlackRank + ")" + "\t\t White: " + White + " (" + WhiteRank + ")" + "\n" +
                    "Ruleset: " + Ruleset + "\t\t Date: " + Date + "\n" +
                    "Komi: " + Komi + " \t\t\t\t        Result: " + Result;
            }
        }

        public void ReadSgf(string sgf)
        {
            var split = sgf.Split('\n');

            var chat = "";
            var chatActive = false;

            foreach (var line in split) //Only look at valid moves for now.
            {
                //This is a really dumb way to parse it but sgf is simple so I can get away with it.

                if (chatActive) //Reading chat lines.
                {
                    chat += line.TrimEnd(new[] {'\r',']'});

                    if (ContainsNotEscaped(']', line))
                    {
                        chatActive = false;
                        var m = new Move(true) {MType = Move.Type.Chat};
                        m.Message = chat;
                        chat = "";
                        Moves.Add(m);
                    }
                }
                    
                if (line.Length < 2)
                    continue;
                var firstTwo = line.Substring(0, 2);
                switch (firstTwo)
                {
                    case ";B": //Black move
                        Moves.Add(LettersToMove(line.Substring(3, 2), Stone.Black));
                        break;
                    case ";W": //White Move
                        Moves.Add(LettersToMove(line.Substring(3, 2), Stone.White));
                        break;
                    case "AB": //Handicap stones.
                        GrabHandicapStones(line);
                        break;
                    case "C[": //Chat message
                        chatActive = true;
                        chat = line.Substring(2, line.Length - 2);
                        break;
                    case "PB":
                        Info.Black = GrabData(line)[0];
                        break;
                    case "BR":
                        Info.BlackRank = GrabData(line)[0];
                        break;
                    case "PW":
                        Info.White = GrabData(line)[0];
                        break;
                    case "WR":
                        Info.WhiteRank = GrabData(line)[0];
                        break;
                    case "SZ":
                        Info.Size = GrabData(line)[0];
                        break;
                    case "KM":
                        Info.Komi = GrabData(line)[0];
                        break;
                    case "RE":
                        Info.Result = GrabData(line)[0];
                        break;
                    case "HA":
                        Info.Handicap = GrabData(line)[0];
                        break;
                    case "RU":
                        Info.Ruleset = GrabData(line)[0];
                        break;
                    case "DT":
                        Info.Date = GrabData(line)[0];
                        break;
                    case "PC":
                        Info.Link = GrabData(line)[0];
                        break;
                }

            }

        }

        private static Move LettersToMove(string letter, Stone colour)
        {
            var x = letter[0] % 32;
            var y = letter[1] % 32;
            return new Move(colour, x, y);
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
           
            foreach (var s in strs)
                Moves.Add(LettersToMove(s, Stone.Black));
        }

        public void PlaceAllSGFMoves()
        {
            foreach (var m in Moves)
            {
                switch (m.MType)
                {
                    case Move.Type.Chat:
                        break;

                    case Move.Type.Place:
                        PlaceStone(m);
                        break;
                }
            }
        }
    }
}


