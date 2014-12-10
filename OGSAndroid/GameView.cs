using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace OGSAndroid
{
    class GameView : SGFView
    {
        public GameView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            boardTouch.OnConfirmStone += boardTouch_OnConfirmStone;
        }

        private static void boardTouch_OnConfirmStone(Stone stone)
        {
            RealTimeAPI.I.Move((Move)stone);
        }


    }
}