using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OGSAndroid
{
    public class Stone //Can be used statically like an enum.
    {     
        public static readonly Stone Black = new Stone(true);
        public static readonly Stone White = new Stone(false);

        public int x;
        public int y;
        readonly bool val;

        public Stone(bool _val) { val = _val;}
        public Stone(bool _val, int _x, int _y) { val = _val; x = _x; y = _y; }
        public static implicit operator bool (Stone s) { return s.val; }

        public Stone[] AdjacentStones(int boardSize)
        {
            var sts = new List<Stone>();
            if (x != 1) sts.Add(new Stone(true,x-1,y));
            if(y != 1) sts.Add(new Stone(true,x,y-1));
            if(x < boardSize) sts.Add(new Stone(true, x+1,y));
            if(y < boardSize) sts.Add(new Stone(true, x, y + 1));
            return sts.ToArray();
        }

        public Stone Clone()
        {
            return new Stone(this.val, x, y);
        }

        public bool EqualsExact(Stone st)
        {
            return st.x == this.x && st.y == this.y && st.val == val;
        }

        public static bool CollectionContainsExact(Stone st, IEnumerable<Stone> coll)
        {
            foreach (var s in coll)
            {
                if (s.EqualsExact(st))
                    return true;
            }
            return false;
        }

        public bool GroupAlive(List<Stone> board, int boardSize, out Stone[] stgrp)
        {
            var adj = this.AdjacentStones(boardSize);

            bool alive = false;

            List<Stone> grp = new List<Stone>();
            grp.Add(this);

            Queue<Stone> workGrp = new Queue<Stone>();
            foreach (var adjst in adj)
                workGrp.Enqueue(adjst);

            while(workGrp.Count > 0)
            {
                var s = workGrp.Dequeue();

                if (CollectionContainsExact(s, grp))
                    continue;

                s = GetStone(s.x, s.y, board);
                if (s == null)
                {

                    alive = true;
                    continue;
                }
                if (s.val == this.val)//Same colour
                {
                    grp.Add(s);
                    foreach (var adjStone in s.AdjacentStones(boardSize))
                        workGrp.Enqueue(adjStone);
                }
            }

            stgrp = grp.ToArray();

            return alive;

        }

        public static Stone GetStone(int x, int y, List<Stone> board)
        {

            return board.FirstOrDefault(st => st.x == x && st.y == y);
        }

        public static bool StoneAlreadyExists(Stone s, List<Stone> board)
        {
            return board.Any(st => s.x == st.x && s.y == st.y);
        }
    }
}

