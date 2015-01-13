#region

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OGSAndroid.Game;
using Quobject.EngineIoClientDotNet.Client;
using Quobject.SocketIoClientDotNet.Client;
using Socket = Quobject.SocketIoClientDotNet.Client.Socket;

#endregion

namespace OGSAndroid.API
{
    internal class RealTimeAPI
    {
        public delegate void IncomingMessageDelegate(JObject data);

        //Singleton.
        public static RealTimeAPI I = new RealTimeAPI();
        private bool connected;
        private bool connecting;
        public APIInfo Info = new APIInfo();
        private Socket ogsSocket;
        public IncomingMessageDelegate OnGameClock;
        public IncomingMessageDelegate OnGameData;
        public IncomingMessageDelegate OnGameMove;
        //Incoming messages.

        private readonly List<string> registeredMessages = new List<string>();

        public RealTimeAPI()
        {
            Turn = Stone.Black;
        }

        public Stone Turn { get; private set; }

        public void Start(bool authed, string username = null)
        {
            if (connecting) return;

            connecting = true;

            var url = OGSAPI.I.Beta ? "http://ggsbeta.online-go.com/" : "http://ggs.online-go.com/";
            ogsSocket = IO.Socket(url);
            RegisterErrorMessages();
            RegisterIncomingMessages();
            ogsSocket.On(Socket.EVENT_CONNECT, () =>
            {
                connected = true;
                connecting = false;
                ALog.Info("Socket", "OGSSocket connected.");
            });

            if (authed && username != null)
            {
                I.Info.PlayerID = OGSAPI.I.GetPlayerID(username);
                I.Info.PlayerUsername = username;
                I.Info.AuthedUser = true;
            }

            ALog.Info("Socket", "Started");
        }

        public void Stop()
        {
            ogsSocket.Close();
            connected = false;
        }

        //Connect to game.
        public void Connect(string gid)
        {
            Info.GameID = gid;

            var jObj = new JObject
            {
                {"game_id", gid},
                {"player_id", Info.PlayerID},
                {"chat", false}
            };

            RegisterIncomingMessages();

            if (OGSAPI.I.Authed)
                Info.GameAuth = OGSAPI.I.GetGameAuth(gid);

            ogsSocket.Emit("game/connect", jObj);

            ALog.Info("Socket", "emit game/connect : " + gid);
        }

        //Disconnect from current game.
        public void Disconnect()
        {
            var jObj = new JObject
            {
                {"game_id", Info.GameID}
            };

            ogsSocket.Emit("game/disconnect", jObj);

            ALog.Info("Socket", "emit game/disconnect : " + Info.GameID);
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
        public void Move(Stone mv)
        {
            if (!OGSAPI.I.Authed)
                return;

            var jObj = new JObject
            {
                {"auth", Info.GameAuth},
                {"game_id", Info.GameID},
                {"player_id", Info.PlayerID},
                {"move", mv.ToXYString(false)}
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

        private void RegisterIncomingMessages()
        {
            foreach (var o in registeredMessages)
            {
                ogsSocket.Off(o);
            }
            registeredMessages.Clear();

            //Incoming gamedata - 0
            if (Info.GameID == null)
                return;

            var gameData = "game/" + Info.GameID + "/gamedata";
            ogsSocket.On(gameData, data =>
            {
                var json = JObject.Parse(data.ToString());
                if (OnGameData != null)
                    OnGameData.Invoke(json);

                ALog.Info("Socket", data.ToString());
            });
            registeredMessages.Add(gameData);


            var gameMove = "game/" + Info.GameID + "/move";
            ogsSocket.On(gameMove, data =>
            {
                var json = JObject.Parse(data.ToString());
                if (OnGameMove != null)
                    OnGameMove.Invoke(json);
            });
            registeredMessages.Add(gameMove);

            ALog.Info("Socket", "Registered incoming messages");
        }

        private void RegisterErrorMessages()
        {
            ogsSocket.On(Socket.EVENT_CONNECT_ERROR,
                data => { ALog.Info("SocketError", data.ToString() + ":: Connect_Error"); });

            ogsSocket.On(Socket.EVENT_CONNECT_TIMEOUT,
                data => { ALog.Info("SocketError", data.ToString() + ":: Connect_Error_Timeout"); });
            ogsSocket.On(Socket.EVENT_ERROR,
                data => { ALog.Info("SocketError", ((EngineIOException) data).Message + ":: Error"); });
            ogsSocket.On(Socket.EVENT_DISCONNECT,
                data => { ALog.Info("SocketError", data.ToString() + ":: Disconnect"); });

            ogsSocket.On(Socket.EVENT_MESSAGE, data => { ALog.Info("SocketMessage", data.ToString() + ":: Message"); });
        }

        public class APIInfo
        {
            public string GameID { get; set; }
            public string PlayerID { get; set; }
            public string PlayerUsername { get; set; }
            public bool AuthedUser { get; set; }
            public string ChatID { get; set; }
            public string AuthID { get; set; }
            public string GameAuth { get; set; }
        }
    }
}