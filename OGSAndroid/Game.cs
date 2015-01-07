using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using OGSAndroid;

namespace OGSAndroid
{
    class Game : SGF<Move>
    {

        public void PopulateViaGameObject(JObject j)
        {
            Info.Black = j["black"]["username"].ToString();
            Info.BlackRank = j["black"]["rank"].ToString();
            Info.White = j["white"]["username"].ToString();
            Info.WhiteRank = j["white"]["rank"].ToString();

            var h = j["height"].ToString();
            var w = j["width"].ToString();

            if (h != w)
            {
                ALog.Info("MatchInfo", "Width/Height are not the same, not yet supported.");
                throw new NotImplementedException("width/height must be the same.");
            }

            Info.Size = w;
            Info.Handicap = j["handicap"].ToString();

           
        }
    }
}