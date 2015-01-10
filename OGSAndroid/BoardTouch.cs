#region

using System;
using System.Linq;
using Android.Views;
using Newtonsoft.Json.Bson;
using Math = System.Math;
using Object = Java.Lang.Object;

#endregion

namespace OGSAndroid
{
    public class BoardTouch : Object, View.IOnTouchListener
    {
        public delegate void StonePlaceDelegate(Stone stone, MotionEvent e);
        public delegate void TouchDelegate(MotionEvent e);
        
        public enum ConfirmType { DoubleTap, Sumbit }
        public ConfirmType SubmitType = ConfirmType.Sumbit;

        public event StonePlaceDelegate OnConfirmStone;
        public event StonePlaceDelegate OnPlaceStone;
        public event TouchDelegate OnTouchEvent;

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
            OnTouchEvent(e);

            if (e.Action != MotionEventActions.Down && e.Action != MotionEventActions.Move) return true;

            if (e.Action == MotionEventActions.Move)
            {
                Console.WriteLine("Move");
            }

            float[] xy = {e.GetX(), e.GetY()};
            int padding = view.Padding, spacing = view.Spacing;
            var spacingsmall = spacing/2;
            var xyi = new int[2];
            xyi[0] = (int) Math.Ceiling(((xy[0] - padding) + spacingsmall)/spacing);
            xyi[1] = (int) Math.Ceiling(((xy[1] - padding) + spacingsmall)/spacing);

            if (xyi.SequenceEqual(ConfirmStonePos) && SubmitType == ConfirmType.DoubleTap)
            {
                SubmitMove(e);
            }
            else
            {
                ConfirmStone = new Stone(view.CurrentTurn, xyi[0], xyi[1]);
                ConfirmStonePos = xyi;
                ConfirmStoneActive = true;
                view.Invalidate();

                if (OnConfirmStone != null)
                    OnConfirmStone.Invoke(ConfirmStone, e);
            }

            return true; //Do not consume.
        }

        public void SubmitMove(MotionEvent e)
        {
            if (!ConfirmStoneActive) return;
            ConfirmStoneActive = false;
            view.PlaceStone(new Stone(view.CurrentTurn, ConfirmStonePos[0], ConfirmStonePos[1]));
            if(OnPlaceStone != null)
                OnPlaceStone.Invoke(ConfirmStone, e);
        }

        public void Reset()
        {
            ConfirmStoneActive = false;
            ConfirmStonePos = new int[2];
        }
    }
}