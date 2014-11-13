using System;

namespace OGSAndroid
{
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


    public class SGF<T>
    {
        public MatchInfo Info {get; set;}
        public SGFTree<T> Tree {get; set;}

        public SGF()
        {
        	Tree = new SGFTree<T>();
        }
    }
}

