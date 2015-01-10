using System;

namespace OGSAndroid
{
    public class ALog
    {
        public static void Info(string tag, string msg)
        {
            Android.Util.Log.Info(" OGSALOG || " + tag, msg);
        }
    }
}

