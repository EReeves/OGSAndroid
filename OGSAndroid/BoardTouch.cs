#region

using System;
using System.Linq;
using Android.Views;
using Math = System.Math;
using Object = Java.Lang.Object;

#endregion

namespace OGSAndroid
{
    public class BoardTouch : Object, View.IOnTouchListener
    {
        private readonly BoardView view;

        public event Action OnConfirmStone;

        public bool ConfirmStoneActive { get; private set; }
        public Stone ConfirmStone { get; private set; }
        public int[] ConfirmStonePos { get; private set; }

        public BoardTouch(BoardView _view)
        {
            view = _view;
            _view.SetOnTouchListener(this);

            ConfirmStoneActive = false;
            ConfirmStonePos = new int[2];
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (e.Action != MotionEventActions.Down) return false;

            float[] xy = {e.GetX(), e.GetY()};
            int padding = view.Padding, spacing = view.Spacing;
            int spacingsmall = spacing/2;
            var xyi = new int[2];
            xyi[0] = (int) Math.Ceiling(((xy[0] - padding) + spacingsmall)/spacing);
            xyi[1] = (int) Math.Ceiling(((xy[1] - padding) + spacingsmall)/spacing);

            Console.Write("x: " + xyi[0] + " y: " + xyi[1] + "\n");

            if (xyi.SequenceEqual(ConfirmStonePos))
            {
                ConfirmStoneActive = false;
                view.PlaceStone(new Stone(view.CurrentTurn, xyi[0], xyi[1]));
            }
            else
            {
                ConfirmStone = new Stone(view.CurrentTurn, xyi[0], xyi[1]);
                ConfirmStonePos = xyi;
                ConfirmStoneActive = true;
                view.Invalidate();

                if(OnConfirmStone != null)
                    OnConfirmStone.Invoke();
            }

            return false; //Do not consume.
        }

        public void Reset()
        {
            ConfirmStoneActive = false;
            ConfirmStonePos = new int[2];
        }
    }
}