#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace OGSAndroid.Game
{
    public class SGFParser
    {
        private readonly Stack<Node<Stone>> currPos = new Stack<Node<Stone>>();
        private int sgfCounter;
        private TreeState state = TreeState.Continue;

        public SGF<Stone> Parse(string sgf)
        {
            var temp = new SGF<Stone>();

            for (sgfCounter = 0; sgfCounter < sgf.Length; sgfCounter++)
            {
                switch (sgf[sgfCounter])
                {
                    case '[':
                        if (sgf[sgfCounter - 1] == '\\')
                            break;
                        temp = SortStone(sgf, sgfCounter, temp);
                        break;
                    case '(':
                        //Create variation on next move.
                        state = TreeState.Variation;
                        break;
                    case ')':
                        //Move back in tree.
                        if (!currPos.Any())
                            break;
                        currPos.Pop();
                        state = TreeState.Continue;
                        break;
                }
            }

            temp.Tree.PopulateNodesList();
            return temp;
        }

        public SGF<Stone> SortStone(string sgf, int bPos, SGF<Stone> temp)
        {
            var type = sgf.Substring(bPos - 2, 2);
            var data = "";

            var i = bPos;
            while (sgf[i + 1] != ']') //Grab data between brackets.
            {
                i++;
                data += sgf[i];
            }

            Console.WriteLine(type + " " + data);

            switch (type)
            {
                case ";B": //Black Stone
                    var m = Stone.LettersToMove(data, Stone.Black);
                    InsertStone(m, ref temp);
                    break;
                case ";W": //White Stone
                    var mw = Stone.LettersToMove(data, Stone.White);
                    InsertStone(mw, ref temp);
                    break;
                case "AB": //Handicap stones. //Todo: WB, white stones.
                    GrabHandicapStones(sgf, bPos, ref temp, Stone.Black);
                    break;
                case "\nC":
                case "C": //Chat message
                    int chatEnd;
                    temp.Info.ChatMessages.Add(GrabData(sgf, bPos, out chatEnd)[0]);
                    sgfCounter = chatEnd;
                    break;
                case "PB":
                    temp.Info.Black = GrabData(sgf, bPos)[0];
                    break;
                case "BR":
                    temp.Info.BlackRank = GrabData(sgf, bPos)[0];
                    break;
                case "PW":
                    temp.Info.White = GrabData(sgf, bPos)[0];
                    break;
                case "WR":
                    temp.Info.WhiteRank = GrabData(sgf, bPos)[0];
                    break;
                case "SZ":
                    temp.Info.Size = GrabData(sgf, bPos)[0];
                    break;
                case "KM":
                    temp.Info.Komi = GrabData(sgf, bPos)[0];
                    break;
                case "RE":
                    temp.Info.Result = GrabData(sgf, bPos)[0];
                    break;
                case "HA":
                    temp.Info.Handicap = GrabData(sgf, bPos)[0];
                    break;
                case "RU":
                    temp.Info.Ruleset = GrabData(sgf, bPos)[0];
                    break;
                case "DT":
                    temp.Info.Date = GrabData(sgf, bPos)[0];
                    break;
                case "PC":
                    temp.Info.Link = GrabData(sgf, bPos)[0];
                    break;
            }

            return temp;
        }

        public void InsertStone(Stone mv, ref SGF<Stone> sg)
        {
            if (mv == null)
                return; //pass or error

            if (currPos.Count == 0)
            {
                sg.Tree.FirstNode = new Node<Stone>(mv, sg.Tree);
                currPos.Push(sg.Tree.FirstNode);
                return;
            }

            switch (state)
            {
                case TreeState.Continue:
                    currPos.Peek().AddChild(mv);
                    break;

                case TreeState.Variation:
                    var node = currPos.Peek().AddChild(mv);
                    currPos.Push(node);
                    state = TreeState.Continue;
                    break;
            }
        }

        private static string[] GrabData(string sgf, int bPos, out int endpos)
        {
            var data = "";
            var i = bPos;

            while (i == bPos || !(sgf[i] == ']' && sgf[i + 1] != '['))
            {
                data += sgf[i];
                if (i + 1 >= sgf.Length)
                    break;
                i++;
            }

            endpos = i;

            var splt = data.Split(new[] {'[', ']'}, StringSplitOptions.RemoveEmptyEntries);
            var ncr = splt.Where(c => c != "\r");

            return ncr.ToArray();
        }

        private static string[] GrabData(string sgf, int bPos)
        {
            int outDummy;
            return GrabData(sgf, bPos, out outDummy);
        }

        private void GrabHandicapStones(string sgf, int bPos, ref SGF<Stone> sg, Stone colour)
        {
            var ncr = GrabData(sgf, bPos);

            foreach (var m in ncr.Select(mv => Stone.LettersToMove(mv, colour)))
            {
                if (currPos.Count == 0)
                {
                    sg.Tree.FirstNode = new Node<Stone>(m, sg.Tree);
                    currPos.Push(sg.Tree.FirstNode);
                    continue;
                }

                var node = currPos.Peek().AddChild(m);
                currPos.Push(node);
            }
        }

        private enum TreeState
        {
            Continue,
            Variation
        }
    }
}