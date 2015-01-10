#region

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#endregion

namespace OGSAndroid.Game
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
     
        public string PlayerString(Stone colour)
        {
            var name = colour ? Black : White;
            var rank = colour ? BlackRank : WhiteRank;

            return name + " (" + rank + ")";
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

        public SGF<T> PopulateViaGameObject(JObject j)
        {
            this.Info.Black = j["players"]["black"]["username"].ToString();
            this.Info.BlackRank = j["players"]["black"]["rank"].ToString();
            this.Info.White = j["players"]["white"]["username"].ToString();
            this.Info.WhiteRank = j["players"]["white"]["rank"].ToString();

            var h = j["height"].ToString();
            var w = j["width"].ToString();

            if (h != w)
            {
                ALog.Info("MatchInfo", "Width/Height are not the same, not yet supported.");
                throw new NotImplementedException("width/height must be the same.");
            }

            this.Info.Size = w;
            this.Info.Handicap = j["handicap"].ToString();

            return this;
        }

    }
}