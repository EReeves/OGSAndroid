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
                        if (currPos.Count() == 0)
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
                    case "AB": //Handicap stones. //Todo: WB, white stones.
                        GrabHandicapStones(sgf, bPos, ref temp, Stone.Black);
                        break;
                    case "C": //Chat message

                        break;
                    case "PB":
                        temp.Info.Black = GrabData(sgf,bPos)[0];            
                        break;
                    case "BR":
                        temp.Info.BlackRank = GrabData(sgf,bPos)[0]; 
                        break;
                    case "PW":
                        temp.Info.White = GrabData(sgf,bPos)[0]; 
                        break;
                    case "WR":
                        temp.Info.WhiteRank = GrabData(sgf,bPos)[0]; 
                        break;
                    case "SZ":
                        temp.Info.Size = GrabData(sgf,bPos)[0]; 
                        break;
                    case "KM":
                        temp.Info.Komi = GrabData(sgf,bPos)[0]; 
                        break;
                    case "RE":
                        temp.Info.Result = GrabData(sgf,bPos)[0]; 
                        break;
                    case "HA":
                        temp.Info.Handicap = GrabData(sgf,bPos)[0];
                        break;
                    case "RU":
                        temp.Info.Ruleset = GrabData(sgf,bPos)[0]; 
                        break;
                    case "DT":
                        temp.Info.Date = GrabData(sgf,bPos)[0]; 
                        break;
                    case "PC":
                        temp.Info.Link = GrabData(sgf,bPos)[0]; 
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
                    var node = currPos.Peek().AddChild(mv);
                    currPos.Push(node);
                    state = TreeState.Continue;
                    break;
            }

        }

         private string[] GrabData(string sgf, int bPos)
        {
            var data = "";
            var i=bPos;

            while( i == bPos || !(sgf[i] == ']' && sgf[i+1] != '[') )   
            {
                data += sgf[i];
                if (i + 1 >= sgf.Length)
                    break;
                i++;
            }

            var splt = data.Split(new[]{ '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            var ncr = splt.Where(c => c != "\r");       

            return ncr.ToArray();
        }

        private void GrabHandicapStones(string sgf, int bPos, ref SGF<Move> sg, Stone colour)
        {
            var ncr = GrabData(sgf,bPos);

            foreach (var mv in ncr) 
            {
                //Add each handicap stone as new child.
                var m = Move.LettersToMove(mv, colour);

                if (currPos.Count == 0)
                {
                    sg.Tree.FirstNode = new Node<Move>(m, sg.Tree);
                    currPos.Push(sg.Tree.FirstNode);
                    continue;
                }

                var node = currPos.Peek().AddChild(m);
                currPos.Push(node);
            }
        }
    }
}

