﻿#region

using System.Collections.Generic;

#endregion

namespace OGSAndroid
{
    public class MatchInfo
    {
        public string Black;
        public string BlackRank;
        public List<string> ChatMessages = new List<string>();
        public string Date;
        public string Handicap;
        public string ID;
        public string Komi;
        public string Link;
        public string Result;
        public string Ruleset;
        public string Size;
        public string White;
        public string WhiteRank;

        public string String()
        {
            return
                "Black: " + Black + " (" + BlackRank + ")" + "\t\t White: " + White + " (" + WhiteRank + ")" + "\n" +
                "Ruleset: " + Ruleset + "\t\t Date: " + Date + "\n" +
                "Komi: " + Komi + " \t\t\t        Result: " + Result;
        }

        public string LeftString()
        {
            return Black + " (" + BlackRank + ")"; // + "\nRuleset: " + Ruleset + "\nKomi: " + Komi;
        }

        public string RightString()
        {
            return White + " (" + WhiteRank + ")"; // + "\nDate: " + Date + "\nResult: " + Result;
        }
    }


    public class SGF<T>
    {
        public MatchInfo Info;

        public SGF()
        {
            Tree = new SGFTree<T>();
            Info = new MatchInfo();
        }

        public SGFTree<T> Tree { get; set; }
    }
}