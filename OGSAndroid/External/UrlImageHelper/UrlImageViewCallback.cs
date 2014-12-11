#region

using Android.Graphics.Drawables;
using Android.Widget;

#endregion

namespace OGSAndroid.External.UrlImageHelper
{
    public interface IUrlImageViewCallback
    {
        void OnLoaded(ImageView imageView, Drawable loadedDrawable, string url, bool loadedFromCache);
    }
}