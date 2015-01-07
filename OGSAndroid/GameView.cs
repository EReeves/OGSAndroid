using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Newtonsoft.Json.Linq;

namespace OGSAndroid
{
    public class GameView : SGFView
    {
        public GameView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }

        public void Connect(string gid)
        {
            RealTimeAPI.I.Connect(gid);
            RealTimeAPI.I.OnGameData += (d) =>
            {
                ((Game)Moves).PopulateViaGameObject(d);
                PopulateMovesViaGameObject(d);
            };

            Initialize(Convert.ToInt32(Moves.Info.Size));
        }

        public void Disconnect()
        {
            RealTimeAPI.I.OnGameData = null;
        }          

        private void PopulateMovesViaGameObject(JObject json)
        {
            var moves = json["moves"].ToString();
            var turn = Stone.Black;
            for (var i = 0; i < moves.Length; i += 2)
            {
                var stone = new Stone(turn, Convert.ToInt32(moves[i]),  Convert.ToInt32(moves[i + 1]));
                PlaceStone(stone);

                //Swap turns
                turn = new Stone(!turn);
            }

            ToEnd();
        }
    }
}