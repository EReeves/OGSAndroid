#region

using Android.Util;

#endregion

namespace OGSAndroid
{
    public class ALog
    {
        public static void Info(string tag, string msg)
        {
            Log.Info(" OGSALOG || " + tag, msg);
        }
    }
}