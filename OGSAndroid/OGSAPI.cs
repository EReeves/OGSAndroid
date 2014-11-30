#region

using System.Collections.Generic;
using System.IO;
using System.Net;
using SimpleJSON;

#endregion

namespace OGSAndroid
{
    public class OGSAPI
    {
        public static string GetPlayerID(string username)
        {
            string url = "https://online-go.com/api/v1/players?username=" + username;
            JSONNode ds = JsonGet(url);
            
            //Player not found : Player found
            return ds["results"].AsArray[0]["id"] == null ? "" : ds["results"].AsArray[0]["id"].Value;
        }

        public static OGSGame[] PlayerGameList(string id, int page)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var gameList = new List<OGSGame>();
            //Just do the first page for now.

            string url = "http://online-go.com/api/v1/players/" + id + "/games?ordering=-id&page=" + page;
            JSONNode ds = JsonGet(url);
            JSONArray games = ds["results"].AsArray;

            foreach (JSONNode g in games)
            {
                var temp = new OGSGame
                {
                    Name = g["name"].Value,
                    ID = g["id"].Value,
                    Black =
                    {
                        Username = g["players"]["black"]["username"],
                        Id = g["players"]["black"]["id"],
                        Country = g["players"]["black"]["country"],
                        Icon = g["players"]["black"]["icon"]
                    },
                    White =
                    {
                        Username = g["players"]["white"]["username"],
                        Id = g["players"]["white"]["id"],
                        Country = g["players"]["white"]["country"],
                        Icon = g["players"]["white"]["icon"]
                    }
                };

                //Find result.
                bool white = g["black_lost"].AsBool;
                string outcome = g["outcome"].Value;

                if (white)
                    temp.Result = "W+" + outcome;
                else
                    temp.Result = "B+" + outcome;

                //Change icon src size to 256px.
                temp.Black.Icon = temp.Black.Icon.Replace("32.png", "256.png");
                temp.White.Icon = temp.White.Icon.Replace("32.png", "256.png");

                gameList.Add(temp);
            }

            return gameList.ToArray();
        }

        private static JSONNode JsonGet(string url)
        {
            var wr = WebRequest.Create(url);
            string json = WebRequestWrapper(url);

            return JSON.Parse(json);
        }

        public static SGF<Move> IDToSGF(string gid)
        {
            var parser = new SGFParser();
            return parser.Parse(DownloadSGF(gid));
        }

        private static string DownloadSGF(string gid)
        {
            string url = "http://online-go.com/api/v1/games/" + gid + "/sgf";
            return WebRequestWrapper(url);
        }

        private static string WebRequestWrapper(string url)
        {
            var wr = WebRequest.Create(url);
            string result;

            using (var hr = (HttpWebResponse)wr.GetResponse())
            using (var stream = hr.GetResponseStream())
            using (var reader = new StreamReader(stream))
                result = reader.ReadToEnd();

            return result;
        }
    }
}