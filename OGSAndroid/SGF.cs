#region

using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

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
        public Stone PlayerColour = null;       

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