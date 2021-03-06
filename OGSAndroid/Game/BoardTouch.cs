#region

using Android.Views;
using Java.Lang;
using Math = System.Math;

#endregion

namespace OGSAndroid.Game
{
    public class BoardTouch : Object, View.IOnTouchListener
    {
        public delegate void StonePlaceDelegate(Stone stone, MotionEvent e);

        public delegate void TouchDelegate(MotionEvent e);

        private readonly BoardView view;

        public BoardTouch(BoardView _view)
        {
            view = _view;
            _view.SetOnTouchListener(this);

            ConfirmStoneActive = false;
            ConfirmStonePos = new int[2];
        }

        public bool ConfirmStoneActive { get; set; }
        public Stone ConfirmStone { get; private set; }
        public int[] ConfirmStonePos { get; private set; }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (OnTouchEvent != null) OnTouchEvent(e);

            if (e.Action != MotionEventActions.Down && e.Action != MotionEventActions.Move)
                return true; //Not a touch we need to worry about.

            //Get x/y position of stone.
            float[] xy = {e.GetX(), e.GetY()};
            int padding = view.Padding, spacing = view.Spacing;
            var spacingsmall = spacing/2;
            var xyi = new int[2];
            xyi[0] = (int) Math.Ceiling(((xy[0] - padding) + spacingsmall)/spacing);
            xyi[1] = (int) Math.Ceiling(((xy[1] - padding) + spacingsmall)/spacing);

            ConfirmStone = new Stone(view.CurrentTurn, xyi[0], xyi[1]);
            ConfirmStonePos = xyi;
            ConfirmStoneActive = true;
            view.Invalidate();

            if (OnConfirmStone != null)
                OnConfirmStone.Invoke(ConfirmStone, e);

            return true; //Do not consume.
        }

        public event StonePlaceDelegate OnConfirmStone;
        public event StonePlaceDelegate OnPlaceStone;
        public event TouchDelegate OnTouchEvent;

        public void SubmitMove(MotionEvent e)
        {
            if (!ConfirmStoneActive) return;
            ConfirmStoneActive = false;
            if (OnPlaceStone != null)
                OnPlaceStone.Invoke(ConfirmStone, e);
        }

        public void SubmitMove()
        {
            SubmitMove(null);
        }

        public void Reset()
        {
            ConfirmStoneActive = false;
            ConfirmStonePos = new int[2];
        }
    }
}