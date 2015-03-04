using System;
using Android.Views;

namespace OGSAndroid
{
    public class OnEnterEditTextListener : Java.Lang.Object, View.IOnKeyListener
    {
        public Action Action {get;set;}

        public bool OnKey(View v, Keycode keyCode, KeyEvent e)
        {
            if (e.Action == KeyEventActions.Down && keyCode == Keycode.Enter)
            {
                if (Action != null)
                    Action.Invoke();
                return true;
            }
            return false;
        }
            
    }
}

