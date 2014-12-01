#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace OGSAndroid
{
    public class SGFParser
    {
        private readonly Stack<Node<Move>> currPos = new Stack<Node<Move>>();
        private TreeState state = TreeState.Continue;

        private int sgfCounter;

        public SGF<Move> Parse(string sgf)
        {
            var temp = new SGF<Move>();

            for (sgfCounter = 0; sgfCounter < sgf.Length; sgfCounter++)
            {
                switch (sgf[sgfCounter])
                {
                    case '[':
                        if (sgf[sgfCounter - 1] == '\\')
                            break;
                        temp = SortMove(sgf, sgfCounter, temp);
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

        public SGF<Move> SortMove(string sgf, int bPos, SGF<Move> temp)
        {
            string type = sgf.Substring(bPos - 2, 2);
            string data = "";

            int i = bPos;
            while (sgf[i + 1] != ']') //Grab data between brackets.
            {
                i++;
                data += sgf[i];
            }

            Console.WriteLine(type + " " + data);

            switch (type)
            {
                case ";B": //Black move
                    var m = Move.LettersToMove(data, Stone.Black);
                    InsertMove(m, ref temp);
                    break;
                case ";W": //White Move
                    var mw = Move.LettersToMove(data, Stone.White);
                    InsertMove(mw, ref temp);
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

        public void InsertMove(Move mv, ref SGF<Move> sg)
        {
            if (mv == null)
                return; //pass or error

            if (currPos.Count == 0)
            {
                sg.Tree.FirstNode = new Node<Move>(mv, sg.Tree);
                currPos.Push(sg.Tree.FirstNode);
                return;
            }

            switch (state)
            {
                case TreeState.Continue:
                    currPos.Peek().AddChild(mv);
                    break;

                case TreeState.Variation:
                    Node<Move> node = currPos.Peek().AddChild(mv);
                    currPos.Push(node);
                    state = TreeState.Continue;
                    break;
            }
        }

        private static string[] GrabData(string sgf, int bPos, out int endpos)
        {
            string data = "";
            int i = bPos;

            while (i == bPos || !(sgf[i] == ']' && sgf[i + 1] != '['))
            {
                data += sgf[i];
                if (i + 1 >= sgf.Length)
                    break;
                i++;
            }

            endpos = i;

            string[] splt = data.Split(new[] {'[', ']'}, StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<string> ncr = splt.Where(c => c != "\r");

            return ncr.ToArray();
        }

        private static string[] GrabData(string sgf, int bPos)
        {
            int outDummy;
            return GrabData(sgf, bPos, out outDummy);
        }

        private void GrabHandicapStones(string sgf, int bPos, ref SGF<Move> sg, Stone colour)
        {
            string[] ncr = GrabData(sgf, bPos);

            foreach (Move m in ncr.Select(mv => Move.LettersToMove(mv, colour)))
            {
                if (currPos.Count == 0)
                {
                    sg.Tree.FirstNode = new Node<Move>(m, sg.Tree);
                    currPos.Push(sg.Tree.FirstNode);
                    continue;
                }

                Node<Move> node = currPos.Peek().AddChild(m);
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