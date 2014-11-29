using System;
using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Animation;
using Android.Views.Animations;
using Android.Gestures;

namespace OGSAndroid
{
    public class ChatDrawer : Java.Lang.Object, GestureDetector.IOnGestureListener, View.IOnTouchListener
    {
        private GestureDetector gDetector;
        private const int VELOCITY_MAXMIN = 200;
        private SlidingDrawer drawer;
        private TextView chatText;

        public bool Open { get { return drawer.IsOpened; } private set { } }
        public GestureDetector GestureDetector { get { return gDetector; } private set { } }

        public string ChatText
        {
            get
            {
                return chatText.Text;
            }
            set
            {
                chatText.Text = value;
            }
        }

        public ChatDrawer(SlidingDrawer _drawer, TextView _chatText)
        {
            drawer = _drawer;
            chatText = _chatText;

            gDetector = new GestureDetector(this);

            drawer.AnimationStart += (object sender, Animation.AnimationStartEventArgs e) => e.Animation.ScaleCurrentDuration(10000); //Does this not work?

        }

        public static string StringListToString(List<String> l)
        {
            if (l == null)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
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

        public bool OnFling(MotionEvent one, MotionEvent two, float vX, float vY)
        {
            if (Math.Abs(vY) > VELOCITY_MAXMIN*4) //Probably scrolling, don't animate.
                return false;

            if (!drawer.IsOpened && vX < -VELOCITY_MAXMIN)
            {
                drawer.AnimateOpen();
            }
            else if (drawer.IsOpened && vX > VELOCITY_MAXMIN)
            {
                drawer.AnimateClose();
            }

            return false;
        }      

        public bool OnTouch(View v, MotionEvent e)
        { 
            OnTouchEvent(e);
            return false;
        }

        public bool OnTouchEvent(MotionEvent e)
        {
            gDetector.OnTouchEvent(e);
            return false;
        }


        //We don't care about these.
        public bool OnDown(MotionEvent e) { return false; }       
        public void OnLongPress(MotionEvent e) {}
        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)  { return false; }      
        public void OnShowPress(MotionEvent e) {}
        public bool OnSingleTapUp(MotionEvent e) { return false; }

    }

}

