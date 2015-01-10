using System;
using Android.Content;
using Android.Util;
using Newtonsoft.Json.Linq;
using OGSAndroid.Activities;
using OGSAndroid.API;

namespace OGSAndroid.Game
{
    public class GameView : SGFView
    {
        public GameView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            RegisterTouchEvents();
        }

        private void RegisterTouchEvents()
        {
            //Only allow placing of stones by the player.
            boardTouch.OnConfirmStone += (stone, e) =>
            {
                var player = GetPlayerColour();
                if (player == null || !stone.Equals(player))
                    boardTouch.ConfirmStoneActive = false;
            };

            //Send move
            boardTouch.OnPlaceStone += (m, e) =>
            {
                RealTimeAPI.I.Move(m.Move());
                Moves.Tree.AddToEnd(m.Move());
            };
        }

        public void Connect(string gid)
        {
            RealTimeAPI.I.Connect(gid);
            RealTimeAPI.I.OnGameData += (d) =>
            {
                ((BoardActivity)Context).RunOnUiThread( () =>
                {
                    Moves = Moves.PopulateViaGameObject(d);
                    Initialize(Convert.ToInt32(Moves.Info.Size));
                    PopulateMovesViaGameObject(d);
                });
            };                   
        }

        public void Disconnect()
        {
            RealTimeAPI.I.Disconnect();
            RealTimeAPI.I.OnGameData = null;
        }          

        private void PopulateMovesViaGameObject(JObject json)
        {
            var moves = json["moves"].ToString();
            var turn = Stone.Black;
            for (var i = 0; i < moves.Length; i += 2)
            {
                //Add move to tree.
                var mv = "" + moves[i] + moves[i + 1];
                var stone = Move.LettersToMove(mv, turn);
                var node = new Node<Move>(stone, Moves.Tree);
                Moves.Tree.AddToEnd(stone);
                //Swap turns
                turn = new Stone(!turn);
            }

            Moves.Tree.PopulateNodesList();
            ToEnd();
        }

        //Gets the player colour you joined as, or null if spectating.
        private Stone GetPlayerColour()
        {
            if (RealTimeAPI.I.Info.PlayerUsername == Moves.Info.Black)
                return Stone.Black;
            else if (RealTimeAPI.I.Info.PlayerUsername == Moves.Info.White)
                return Stone.White;
            else return null;
        }
    }
}