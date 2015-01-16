#region

using System;
using Android.Content;
using Android.Util;
using Newtonsoft.Json.Linq;
using OGSAndroid.Activities;
using OGSAndroid.API;

#endregion

namespace OGSAndroid.Game
{
    public class GameView : SGFView
    {
        public Stone CurrentAPIMove = Stone.Black;

        public GameView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            RegisterTouchEvents();
            RegisterAPIEvents();
        }

        private void RegisterTouchEvents()
        {
            //Only allow placing of stones by the player.
            boardTouch.OnConfirmStone += (stone, e) =>
            {
                var player = GetPlayerColour();
                    if (!player.Active || !stone.Equals(player))
                    boardTouch.ConfirmStoneActive = false;
            };

            //Send move
            boardTouch.OnPlaceStone += (m, e) =>
            {
                RealTimeAPI.I.Move(m);
                //Moves.Tree.AddToEnd(m.Move());
            };
        }

        private void RegisterAPIEvents()
        {
            RealTimeAPI.I.OnGameData += d =>
            {
                ((BoardActivity) Context).RunOnUiThread(() =>
                {
                    Moves = Moves.PopulateViaGameObject(d);   
                    Initialize(Convert.ToInt32(Moves.Info.Size));
                    PopulateMovesViaGameObject(d);
                            var last = Moves.Tree.LastNode();
                    if(last == null) return;
                            CurrentAPIMove = new Stone(!((Node<Stone>)last).Data, true);
                });
            };

            RealTimeAPI.I.OnGameMove += d =>
            {
                var letters = d["move"];
                if (letters == null) return;
                var move = Stone.LettersToMove(letters.ToString(), CurrentTurn);

                CurrentAPIMove = new Stone(!move, true);

                ((BoardActivity) Context).RunOnUiThread(() =>
                {
                    Moves.Tree.AddToEnd(move);
                    Next();
                });
            };
                    
        }

        public void Connect(string gid)
        {
            RealTimeAPI.I.Connect(gid);
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
                var stone = Stone.LettersToMove(mv, turn);
                var node = new Node<Stone>(stone, Moves.Tree);
                Moves.Tree.AddToEnd(stone);
                //Swap turns
                turn = new Stone(!turn);
            }

            Moves.Tree.PopulateNodesList();
            ToEnd();
        }

        //Gets the player colour you joined as, or inactive if spectating.
        private Stone GetPlayerColour()
        {
            if (RealTimeAPI.I.Info.PlayerUsername == Moves.Info.Black)
                return new Stone(Stone.Black, true);
            if (RealTimeAPI.I.Info.PlayerUsername == Moves.Info.White)
                return new Stone(Stone.White, true);
            return new Stone(true, false);
        }
    }
}