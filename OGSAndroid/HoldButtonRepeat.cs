using System;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

namespace OGSAndroid
{
    public class HoldButtonRepeat
    {
        private Button button;
        private Timer timer;
        private OnTouchListener onTouchListener;
        public Action Invoke;

        //param name="interval">Invoke interval in ms.</param>
        public HoldButtonRepeat(Button _button, double interval)
        {
            button = _button;

            onTouchListener = new OnTouchListener();
            button.SetOnTouchListener(onTouchListener);

            timer = new Timer(interval);
            timer.AutoReset = false;

            timer.Elapsed += (sender, e) => 
            {
                    if(!onTouchListener.Pressed) 
                    {
                        timer.Stop();
                        return;
                    }
                    if(Invoke != null)                     
                        Invoke.Invoke();
                    timer.Start();
            };

            onTouchListener.Down += () =>
            {
                 timer.Stop();
                 timer.Start();
                 if(Invoke != null)                     
                     Invoke.Invoke();
            };


        }

        private class OnTouchListener : Java.Lang.Object, View.IOnTouchListener 
        {
            public bool Pressed;
            public Action Down;

            public bool OnTouch(View v, MotionEvent e)
            {
                switch (e.Action)
                {
                    case MotionEventActions.Up:
                        Pressed = false;
                        break;
                    case MotionEventActions.Down:
                        Pressed = true;
                        if (Down != null)
                            Down.Invoke();
                        break;
                }

                return false; //return folse otherwise we consume OnTouch.        
            }
        }




    }
}

