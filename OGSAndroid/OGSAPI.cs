using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;
using System.Linq;

namespace OGSAndroid
{
    public class OGSAPI
    {
        public OGSAPI()
        {
        }

        	public static string GetPlayerID(string username)
        	{
                var url = "https://online-go.com/api/v1/players?username=" + username;
                var ds = JsonGet(url);

                if(ds["results"].AsArray[0]["id"] != null)
                {
                    var id = ds["results"].AsArray[0]["id"].Value;
                    return id;
                }
                
                throw new Exception("Player not found:" + username );
            }

            public static OGSGame[] PlayerGameList(string id, int page)
            {
                var gameList = new List<OGSGame>();
                ///Just do the first page for now.

                var url = "http://online-go.com/api/v1/players/" + id + "/games?ordering=-id&page=" + page;
                var ds = JsonGet(url);
                var games = ds["results"].AsArray;

                foreach(JSONNode g in games)
                {
                    OGSGame temp = new OGSGame();
                    temp.Name = g["name"].Value;
                    temp.ID = g["id"].Value;

                    temp.Black.Username = g["players"]["black"]["username"];            
                    temp.Black.Id = g["players"]["black"]["id"];
                    temp.Black.Country = g["players"]["black"]["country"];
                    temp.Black.Icon = g["players"]["black"]["icon"];

                    temp.White.Username = g["players"]["white"]["username"];            
                    temp.White.Id = g["players"]["white"]["id"];
                    temp.White.Country = g["players"]["white"]["country"];
                    temp.White.Icon = g["players"]["white"]["icon"];

                    var white = g["black_lost"].AsBool;
                    var outcome = g["outcome"].Value;

                    if (white)
                        temp.Result = "W+" + outcome;
                    else
                        temp.Result = "B+" + outcome;

                    //Change icon size to 256px.
                    temp.Black.Icon = temp.Black.Icon.Replace("32.png", "256.png");
                    temp.White.Icon = temp.White.Icon.Replace("32.png", "256.png");

                    gameList.Add(temp);
                }

                return gameList.ToArray();
            }

            private static SimpleJSON.JSONNode JsonGet(string url)
            {
                WebRequest wr = WebRequest.Create(url);

                string json;

                using( var hr = (HttpWebResponse)wr.GetResponse() )
                using( var stream = hr.GetResponseStream() )
                using( var reader = new StreamReader(stream) )
                    json = reader.ReadToEnd();

                return SimpleJSON.JSON.Parse(json);
            }

         public static SGF<Move> IDToSGF(string gid)
         {
            SGFParser parser = new SGFParser();
            return parser.Parse(DownloadSGF(gid));          
         }

        private static string DownloadSGF(string gid)
        {
            var url = "http://online-go.com/api/v1/games/" + gid +"/sgf";
            WebRequest wr = WebRequest.Create(url);

            string sgf;

            using( var hr = (HttpWebResponse)wr.GetResponse() )
            using( var stream = hr.GetResponseStream() )
            using( var reader = new StreamReader(stream) )
                sgf = reader.ReadToEnd();

            return sgf;
        }
    }
}

