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
                    "Komi: " + Komi + " \t\t\t        Result: " + Result;
            }

            public string LeftString()
            {
            return Black + " (" + BlackRank + ")";// + "\nRuleset: " + Ruleset + "\nKomi: " + Komi;
            }

            public string RightString()
            {
            return White + " (" + WhiteRank + ")";// + "\nDate: " + Date + "\nResult: " + Result;
            }
        }


    public class SGF<T>
    {
        public MatchInfo Info;
        public SGFTree<T> Tree {get; set;}

        public SGF()
        {
        	Tree = new SGFTree<T>();
            Info = new MatchInfo();
        }
    }
}

