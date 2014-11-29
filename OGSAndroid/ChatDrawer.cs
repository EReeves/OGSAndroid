using System;

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
    public class ChatDrawer : Java.Lang.Object, GestureDetector.IOnGestureListener
    {
    
        private GestureDetector gDetector;
        private const int VELOCITY_MAXMIN = 300;
        private SlidingDrawer drawer;

        public bool Open { get { return drawer.IsOpened; } private set { } }

        public ChatDrawer(SlidingDrawer _drawer)
        {
            drawer = _drawer;
            gDetector = new GestureDetector(this);

        }
    
        public void InvokeMotionEvent(MotionEvent e)
        {
            gDetector.OnTouchEvent(e);
        }

        public bool OnFling(MotionEvent one, MotionEvent two, float vX, float vY)
        {

            if (!drawer.IsOpened && vX < -VELOCITY_MAXMIN)
            {
                drawer.AnimateOpen();
            }
            else if (drawer.IsOpened && vX > VELOCITY_MAXMIN)
            {
                drawer.AnimateClose();
            }

            return false;;
        }

        //We don't care about these.
        public bool OnDown(MotionEvent e) { return false; }       
        public void OnLongPress(MotionEvent e) {}
        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)  { return false; }      
        public void OnShowPress(MotionEvent e) {}
        public bool OnSingleTapUp(MotionEvent e) { return false; }

    }

}

