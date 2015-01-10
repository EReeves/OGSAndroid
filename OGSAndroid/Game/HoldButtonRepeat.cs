#region

using System;
using System.Timers;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;

#endregion

namespace OGSAndroid.Game
{
    public class HoldButtonRepeat
    {
        public Action Invoke;
        private readonly Button button;
        private readonly OnTouchListener onTouchListener;
        private readonly Timer timer;
        //param name="interval">Invoke interval in ms.</param>
        public HoldButtonRepeat(Button _button, double interval)
        {
            button = _button;

            onTouchListener = new OnTouchListener();
            button.SetOnTouchListener(onTouchListener);

            timer = new Timer(interval) {AutoReset = false};

            timer.Elapsed += (sender, e) =>
            {
                if (!onTouchListener.Pressed)
                {
                    timer.Stop();
                    return;
                }
                if (Invoke != null)
                    Invoke.Invoke();
                timer.Start();
            };

            onTouchListener.Down += () =>
            {
                timer.Stop();
                timer.Start();
                if (Invoke != null)
                    Invoke.Invoke();
            };
        }

        private class OnTouchListener : Object, View.IOnTouchListener
        {
            public Action Down;
            public bool Pressed;

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