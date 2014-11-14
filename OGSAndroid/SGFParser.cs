using System;
using System.Collections.Generic;
using System.Linq;

namespace OGSAndroid
{
    public class SGFParser
    {

        private enum TreeState
        {
            Continue,
            Variation
        }

        private TreeState state = TreeState.Continue;
        private readonly Stack<Node<Move>> currPos = new Stack<Node<Move>>(); 

        public SGF<Move> Parse(string sgf)
        {
            var temp = new SGF<Move>();

            for(var i=0;i<sgf.Length;i++)
            {
                switch(sgf[i])
                {
                    case '[':
                        if (sgf[i - 1] == '\\')
                            break;
                        temp = SortMove(sgf, i, temp);
                        break;
                    case '(':
                        //Create variation on next move.
                        state = TreeState.Variation;
                        break;
                    case ')':
                        //Move back in tree.
                        currPos.Pop();
                        state = TreeState.Continue;
                        break;
                }
            }

            return temp;
        }

        public SGF<Move> SortMove(string sgf, int bPos, SGF<Move> temp)
        {
            var type = sgf.Substring(bPos-2,2);
            var data = "";

            var i=bPos;
            while(sgf[i+1] != ']') //Grab data between brackets.
            {
                i++;
                data += sgf[i];
            }

            Console.WriteLine(type + " " + data);

            switch(type)
            {
                    case ";B": //Black move
                        var m = Move.LettersToMove(data, Stone.Black);
                        InsertMove(m, ref temp);
                        break;
                    case ";W": //White Move
                        var mw = Move.LettersToMove(data, Stone.White);
                        InsertMove(mw, ref temp);
                        break;
                    case "AB": //Handicap stones.
                       
                        break;
                    case "C": //Chat message

                        break;
                    case "PB":

                        break;
                    case "BR":

                        break;
                    case "PW":

                        break;
                    case "WR":

                        break;
                    case "SZ":

                        break;
                    case "KM":

                        break;
                    case "RE":

                        break;
                    case "HA":

                        break;
                    case "RU":

                        break;
                    case "DT":

                        break;
                    case "PC":

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
                sg.Tree.AddItem(mv);
                currPos.Push(sg.Tree.First());
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
    }
}

