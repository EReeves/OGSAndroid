#region

using System;
using System.Collections.Generic;
using Android.Util;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;

#endregion

namespace OGSAndroid
{
    internal class RealTimeAPI
    {
        //Singleton.
        public static RealTimeAPI I = new RealTimeAPI();
        private bool connected;
        private bool connecting;
        public APIInfo Info = new APIInfo();
        private Socket ogsSocket;

        public RealTimeAPI()
        {
            Turn = Stone.Black;
        }

        public Stone Turn { get; private set; }

        public void Start()
        {
            if (connecting) return;

            connecting = true;
            IO.Options op = new IO.Options();
            op.Secure = true;
            op.ForceJsonp = true;

            Console.WriteLine("Protocol:" + IO.Protocol);

            ogsSocket = IO.Socket("http://ggs.online-go.com/", op);
            RegisterErrorMessages();
            RegisterIncomingMessages();
            ogsSocket.On(Socket.EVENT_CONNECT, () =>
            {
                connected = true;
                connecting = false;
                Console.WriteLine("OGSSocket connected.");
                //Test TODO: Remove
                RealTimeAPI.I.Connect("1216933");
            });

            //RegisterIncomingMessages();
        }

        public void Stop()
        {
            ogsSocket.Close();
            connected = false;
        }

        //Connect to game.
        public void Connect(string gid)
        {
            //Info.GameID = gid;

            var jObj = new JObject
            {
                {"game_id", gid},
                {"player_id", Info.PlayerID},
                {"chat", false}
            };

            ogsSocket.Emit("game/connect", jObj);
        }

        //Disconnect from current game.
        public void Disconnect()
        {
            var jObj = new JObject
            {
                {"game_id", Info.GameID}
            };

            ogsSocket.Emit("game/disconnect", jObj);
        }

        //When creating an open challenge you will be handed a game_id which you can use to call game/wait with.
        public void Wait()
        {
            var jObj = new JObject
            {
                {"game_id", Info.GameID}
            };

            ogsSocket.Emit("game/wait", jObj);
        }

        //Make a move
        public void Move(Move mv)
        {
            var jObj = new JObject
            {
                {"auth", Info.AuthID},
                {"game_id", Info.GameID},
                {"player_id", Info.PlayerID},
                {"move", mv.ToXYString()}
            };

            ogsSocket.Emit("game/move", jObj);
        }

        public void ContitionalMovesSet()
        {
            //TODO: implement
        }

        public void RemovedStonesSet()
        {
            //TODO: implement
        }

        public void RemovedStonesAccept()
        {
            //TODO: implement
        }

        public void RemovedStonesReject()
        {
            //TODO: implement
        }

        public void Cancel()
        {
            var jObj = new JObject
            {
                {"auth", Info.AuthID},
                {"game_id", Info.GameID},
                {"player_id", Info.PlayerID}
            };

            ogsSocket.Emit("game/cancel", jObj);
        }

        public void Annul()
        {
            var jObj = new JObject
            {
                {"auth", Info.AuthID},
                {"game_id", Info.GameID},
                {"player_id", Info.PlayerID}
            };

            ogsSocket.Emit("game/annul", jObj);
        }

        public void UndoRequest(int moveno)
        {
            var jObj = new JObject
            {
                {"auth", Info.AuthID},
                {"game_id", Info.GameID},
                {"player_id", Info.PlayerID},
                {"move_number", moveno}
            };

            ogsSocket.Emit("game/undo/request", jObj);
        }

        public void UndoAccept(int moveno)
        {
            var jObj = new JObject
            {
                {"auth", Info.AuthID},
                {"game_id", Info.GameID},
                {"player_id", Info.PlayerID},
                {"move_number", moveno}
            };

            ogsSocket.Emit("game/undo/accept", jObj);
        }

        public void Pause()
        {
            var jObj = new JObject
            {
                {"auth", Info.AuthID},
                {"game_id", Info.GameID},
                {"player_id", Info.PlayerID}
            };

            ogsSocket.Emit("game/pause", jObj);
        }

        public void Resume()
        {
            var jObj = new JObject
            {
                {"auth", Info.AuthID},
                {"game_id", Info.GameID},
                {"player_id", Info.PlayerID}
            };

            ogsSocket.Emit("game/resume", jObj);
        }

        public void Chat()
        {
            //TODO: Implement
        }

        public class APIInfo
        {
            public string GameID { get; set; }
            public string PlayerID { get; set; }
            public string ChatID { get; set; }
            public string AuthID { get; set; }
        }

        //Incoming messages.

        private readonly Dictionary<int,string> registeredMessages = new Dictionary<int, string>(); 

        private void RegisterIncomingMessages()
        {
            //Incoming gamedata - 0
            var gamedata = "game/" + Info.GameID + "/gamedata";
            ogsSocket.On(gamedata, (data) =>
            {
                Log.Info("On/Gamedata", data.ToString());

            });
            registeredMessages.Add(0,gamedata);

            ogsSocket.On(Socket.EVENT_MESSAGE, (data) =>
                {
                    Log.Info("message", data.ToString());
                    Console.WriteLine(data);
                });

        }

        private void RegisterErrorMessages()
        {

            ogsSocket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                Log.Info("SocketError", data.ToString() + ":: Connect_Error");
            });

            ogsSocket.On(Socket.EVENT_CONNECT_TIMEOUT, (data) =>
            {
                Log.Info("SocketError", data.ToString() + ":: Connect_Error_Timeout");
            });
            ogsSocket.On(Socket.EVENT_ERROR, (data) =>
            {
                Log.Info("SocketError", data.ToString() + ":: Error");
            });
            ogsSocket.On(Socket.EVENT_DISCONNECT, (data) =>
            {
                Log.Info("SocketError", data.ToString() + ":: Disconnect");
            });

        }


    }
}