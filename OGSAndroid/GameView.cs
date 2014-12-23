using Android.Content;
using Android.Util;
using Android.Views;

namespace OGSAndroid
{
    internal class GameView : SGFView
    {
        public GameView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            boardTouch.OnConfirmStone += boardTouch_OnConfirmStone;
        }

        private static void boardTouch_OnConfirmStone(Stone stone, MotionEvent e)
        {
            RealTimeAPI.I.Move((Move) stone);
        }
    }
}