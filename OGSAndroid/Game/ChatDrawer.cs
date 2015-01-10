#region

using System.Collections.Generic;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using Math = System.Math;
using String = System.String;
using StringBuilder = System.Text.StringBuilder;

#endregion

namespace OGSAndroid.Game
{
    public class ChatDrawer : Object, GestureDetector.IOnGestureListener, View.IOnTouchListener
    {
        private const int VelocityMaxmin = 100;
        private readonly TextView chatText;
        private readonly SlidingDrawer drawer;
        private readonly GestureDetector gDetector;

        public ChatDrawer(SlidingDrawer _drawer, TextView _chatText)
        {
            drawer = _drawer;
            chatText = _chatText;

            gDetector = new GestureDetector(this);

            drawer.AnimationStart +=
                (object sender, Animation.AnimationStartEventArgs e) => e.Animation.ScaleCurrentDuration(10000);
            //Does this not work?
        }

        public bool Open
        {
            get { return drawer.IsOpened; }
            private set { }
        }

        public GestureDetector GestureDetector
        {
            get { return gDetector; }
            private set { }
        }

        public string ChatText
        {
            get { return chatText.Text; }
            set { chatText.Text = value; }
        }

        public bool OnFling(MotionEvent one, MotionEvent two, float vX, float vY)
        {
            if (Math.Abs(vY) > VelocityMaxmin*15) //Probably scrolling, don't animate.
                return false;

            if (!drawer.IsOpened && vX < -VelocityMaxmin)
            {
                drawer.AnimateOpen();
            }
            else if (drawer.IsOpened && vX > VelocityMaxmin)
            {
                drawer.AnimateClose();
            }

            return false;
        }

        //We don't care about these.
        public bool OnDown(MotionEvent e)
        {
            return false;
        }

        public void OnLongPress(MotionEvent e)
        {
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public void OnShowPress(MotionEvent e)
        {
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            OnTouchEvent(e);
            return true;
        }

        public static string StringListToString(List<String> l)
        {
            if (l == null)
                return string.Empty;
            var sb = new StringBuilder();
            foreach (var str in l)
            {
                sb.Append(str);
            }
            return sb.ToString();
        }

        public void InvokeMotionEvent(MotionEvent e)
        {
            OnTouchEvent(e);
        }

        public bool OnTouchEvent(MotionEvent e)
        {
            gDetector.OnTouchEvent(e);
            return false;
        }
    }
}