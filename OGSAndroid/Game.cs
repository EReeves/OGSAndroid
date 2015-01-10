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
        public static SGF<Move> PopulateViaGameObject(SGF<Move> mv, JObject j)
        {
            mv.Info.Black = j["players"]["black"]["username"].ToString();
            mv.Info.BlackRank = j["players"]["black"]["rank"].ToString();
            mv.Info.White = j["players"]["white"]["username"].ToString();
            mv.Info.WhiteRank = j["players"]["white"]["rank"].ToString();

            var h = j["height"].ToString();
            var w = j["width"].ToString();

            if (h != w)
            {
                ALog.Info("MatchInfo", "Width/Height are not the same, not yet supported.");
                throw new NotImplementedException("width/height must be the same.");
            }

            mv.Info.Size = w;
            mv.Info.Handicap = j["handicap"].ToString();    

            return mv;
        }
    }
}