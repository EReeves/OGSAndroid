﻿#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Org.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace OGSAndroid
{
    public class OGSAPI
    {
        private static string authToken;

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
            var url = "http://private-anon-e536afebe-ogs.apiary-mock.com/oauth2/access_token";
            var wr = WebRequest.Create(url);
            wr.Method = "POST";

            using(var str = wr.GetRequestStream())
            using (var sw = new StreamWriter(str))
                sw.Write(stringB.ToString());

            using(var response = (HttpWebResponse)wr.GetResponse())
            using(var responseStream = response.GetResponseStream())
            using(var streamReader = new StreamReader(responseStream))
                Console.WriteLine(streamReader.ReadToEnd());

        }

        public static string GetPlayerID(string username)
        {
            string url = "https://online-go.com/api/v1/players?username=" + username;
            JToken ds = JsonGet(url);
            
            //Player not found : Player found
            return ds["results"][0]["id"] == null ? "" : ds["results"][0]["id"].ToString();
        }

        public static OGSGame[] PlayerGameList(string id, int page)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            var gameList = new List<OGSGame>();
            //Just do the first page for now.

            string url = "http://online-go.com/api/v1/players/" + id + "/games?ordering=-id&page=" + page;
            JToken ds = JsonGet(url);
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
                var white = (bool)g["black_lost"];
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
            using(var reader = WebRequestWrapperRaw(url))
            using(var jsonReader = new JsonTextReader(reader))        
                return JToken.ReadFrom(jsonReader);
        }

        public static SGF<Move> IDToSGF(string gid)
        {
            var parser = new SGFParser();
            return parser.Parse(DownloadSGF(gid));
        }

        public void AuthedPost(string url, string content)
        {
            var wr = WebRequest.Create(url);
            wr.Headers.Add("Authorization: Bearer ");
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

        private static StreamReader WebRequestWrapperRaw(string url)
        {
            var wr = WebRequest.Create(url);
            string result;

            var hr = (HttpWebResponse) wr.GetResponse();
            var stream = hr.GetResponseStream();
            var reader = new StreamReader(stream);
            return reader;
        }
    }
}