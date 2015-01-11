#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using OGSAndroid.Game;

#endregion

namespace OGSAndroid.API
{
    public class OGSAPI
    {
        //We only ever want one person authed.
        public static OGSAPI I = new OGSAPI();
        private string accessToken;

        public string AccessToken
        {
            get
            {
                if (accessToken == null) throw new Exception("Not authenticated.");
                return accessToken;
            }
        }

        public void DebugSetAccessToken(string token)
        {
            accessToken = token;
            //TODO:Remove
        }

        public void Authenticate(string clientid, string secret, string user, string pass)
        {
            var stringB = new StringBuilder();
            stringB.Append("client_id=");
            stringB.Append(clientid);
            stringB.Append("\u0026client_secret=");
            stringB.Append(secret);
            stringB.Append("\u0026grant_type=password\u0026username=");
            stringB.Append(user);
            stringB.Append("\u0026password=");
            stringB.Append(pass);

            //Authenticate and store auth token.
            const string url = "https://beta.online-go.com/oauth2/access_token";
            var resp = UnAuthedPost(url, stringB.ToString());
            var json = JObject.Parse(resp);
            accessToken = json["access_token"].ToString();

            ALog.Info("OGSAPI", "Authenticated");
        }

        public string GetPlayerID(string username)
        {
            var url = "http://beta.online-go.com/api/v1/players?username=" + username + "&format=json";
            var ds = JsonGet(url);

            //Player not found : Player found
            var id = ds["results"][0]["id"];
            var playerFound = id != null;

            if (playerFound) return id.ToString();

            ALog.Info("OGSAPI", "Player " + username + " not found.");
            return null;
        }

        //Gets a list of games from a player.
        public OGSGame[] PlayerGameList(string id, int page)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var gameList = new List<OGSGame>();
            //Just do the first page for now.

            var url = "http://beta.online-go.com/api/v1/players/" + id + "/games?ordering=-id&page=" + page;

            JObject json;

            try
            {
                json = JsonGet(url);
            }
            catch (Exception ex)
            {
                ALog.Info("OGSAPI", "Player doesn't have any more pages of games");
                return null;
            }

            var games = json["results"];

            foreach (var g in games.Children())
            {
                var temp = new OGSGame
                {
                    Name = g["name"].ToString(),
                    ID = g["id"].ToString(),
                    Black =
                    {
                        Username = g["players"]["black"]["username"].ToString(),
                        Id = g["players"]["black"]["id"].ToString(),
                        Country = g["players"]["black"]["country"].ToString(),
                        Icon = g["players"]["black"]["icon"].ToString()
                    },
                    White =
                    {
                        Username = g["players"]["white"]["username"].ToString(),
                        Id = g["players"]["white"]["id"].ToString(),
                        Country = g["players"]["white"]["country"].ToString(),
                        Icon = g["players"]["white"]["icon"].ToString()
                    }
                };

                //Calculate rank.
                var bRating = g["players"]["black"]["rating"].Value<float>();
                var wRating = g["players"]["white"]["rating"].Value<float>();

                temp.Black.Rank = RatingToRank(bRating);
                temp.White.Rank = RatingToRank(wRating);

                //Find result.
                var white = (bool) g["black_lost"];
                var outcome = g["outcome"].ToString();

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

        private static string RatingToRank(float rating)
        {
            return rating < 30 ? (30 - rating) + " Kyu" : (rating - 30) + 1 + " Dan";
        }

        private static JObject JsonGet(string url)
        {
            var str = WebRequestWrapper(url);
            return JObject.Parse(str);
        }

        public static SGF<Move> IdToSGF(string gid)
        {
            var parser = new SGFParser();
            return parser.Parse(DownloadSGF(gid));
        }

        public string GetGameAuth(string gid)
        {
            var url = "https://beta.online-go.com/api/v1/games/" + gid;
            var json = AuthedGet(url);
            var j = JObject.Parse(json);
            var gAuth = j["auth"].ToString();
            return gAuth;
        }

        private string AuthedPost(string url, string content)
        {
            var wr = WebRequest.Create(url);
            wr.Headers.Add("Authorization: Bearer " + accessToken);
            wr.Method = "POST";

            using (var str = wr.GetRequestStream())
            using (var sw = new StreamWriter(str))
                sw.Write(content);

            using (var response = (HttpWebResponse) wr.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var streamReader = new StreamReader(responseStream))
                return streamReader.ReadToEnd();
        }

        private string AuthedGet(string url)
        {
            var wr = WebRequest.Create(url);
            wr.Headers.Add("Authorization: Bearer " + accessToken);
            wr.Method = "GET";

            using (var response = (HttpWebResponse) wr.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var streamReader = new StreamReader(responseStream))
                return streamReader.ReadToEnd();
        }

        private static string UnAuthedPost(string url, string content)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            ServicePointManager.ServerCertificateValidationCallback = (s, c, cc, ssl) => true;
            var wr = WebRequest.Create(url);
            wr.Method = "POST";
            wr.ContentType = "application/x-www-form-urlencoded";


            using (var str = wr.GetRequestStream())
            using (var sw = new StreamWriter(str))
            {
                sw.Write(content);
                sw.Flush();
            }


            using (var response = (HttpWebResponse) wr.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var streamReader = new StreamReader(responseStream))
                return streamReader.ReadToEnd();
        }

        private static string DownloadSGF(string gid)
        {
            var url = "beta.online-go.com/api/v1/games/" + gid + "/sgf";
            return WebRequestWrapper(url);
        }

        private static string WebRequestWrapper(string url)
        {
            var wr = WebRequest.Create(url);
            string result;

            using (var hr = (HttpWebResponse) wr.GetResponse())
            using (var stream = hr.GetResponseStream())
            using (var reader = new StreamReader(stream))
                result = reader.ReadToEnd();

            return result;
        }

        private static StreamReader WebRequestWrapperRaw(string url)
        {
            var wr = WebRequest.Create(url);
            var hr = (HttpWebResponse) wr.GetResponse();
            var stream = hr.GetResponseStream();
            var reader = new StreamReader(stream);
            return reader;
        }
    }
}