#region

using Android.Graphics.Drawables;

#endregion

namespace OGSAndroid.External.UrlImageHelper
{
    public class UrlImageCache : SoftReferenceHashTable<string, Drawable>
    {
        private static UrlImageCache instance;

        public static UrlImageCache Instance
        {
            get
            {
                if (instance == null)
                    instance = new UrlImageCache();

                return instance;
            }
        }
    }
}