#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace OGSAndroid
{
    public class OGSAPI
    {
        //We only ever want one person authed.

        private static string accessToken;
        public static string AccessToken
        {
            get
            {
                if(accessToken == null) throw new Exception("Not authenticated.");
                return accessToken;               
            }
        }

        public static void Authenticate(string clientid, string secret, string user, string pass)
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
            const string url = "http://private-anon-e536afebe-ogs.apiary-mock.com/oauth2/access_token";

            var resp = UnAuthedPost(url, stringB.ToString());

            var json = JToken.Parse(resp);

            accessToken = json.Children()["access_token"].ToString();

            //TODO:Parse and set token.
        }

        public static string GetPlayerID(string username)
        {
            var url = "https://online-go.com/api/v1/players?username=" + username;
            var ds = JsonGet(url);

            //Player not found : Player found
            return ds["results"][0]["id"] == null ? "" : ds["results"][0]["id"].ToString();
        }

        public static OGSGame[] PlayerGameList(string id, int page)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var gameList = new List<OGSGame>();
            //Just do the first page for now.

            var url = "http://online-go.com/api/v1/players/" + id + "/games?ordering=-id&page=" + page;
            var ds = JsonGet(url);
            var games = ds["results"];

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

        private static JToken JsonGet(string url)
        {
            using (var reader = WebRequestWrapperRaw(url))
            using (var jsonReader = new JsonTextReader(reader))
                return JToken.ReadFrom(jsonReader);
        }

        public static SGF<Move> IDToSGF(string gid)
        {
            var parser = new SGFParser();
            return parser.Parse(DownloadSGF(gid));
        }

        private static string AuthedPost(string url, string content)
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

        private static string UnAuthedPost(string url, string content)
        {
            var wr = WebRequest.Create(url);
            wr.Method = "POST";

            using (var str = wr.GetRequestStream())
            using (var sw = new StreamWriter(str))
                sw.Write(content);

            using (var response = (HttpWebResponse) wr.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var streamReader = new StreamReader(responseStream))
                return streamReader.ReadToEnd();
        }

        private static string DownloadSGF(string gid)
        {
            var url = "http://online-go.com/api/v1/games/" + gid + "/sgf";
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
            string result;

            var hr = (HttpWebResponse) wr.GetResponse();
            var stream = hr.GetResponseStream();
            var reader = new StreamReader(stream);
            return reader;
        }

        public static void SendMove(Move mv, string id)
        {
            //TODO: before this is implemented anywhere put a timer on it for something crazy like 1 hour until socket api is implemented.
            throw new NotImplementedException();

            if (accessToken == null) Console.WriteLine("Unauthed"); //TODO handle this somehow, not sure yet.

            var url = "http://online-go.com/api/v1/games/" + id + "/move/";
            var content = mv.ToXYString();
            var json = new JObject(new JProperty("move", content));
            AuthedPost(url, json.ToString());
        }
    }
}