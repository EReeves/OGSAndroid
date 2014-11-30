#region

using Android.Graphics.Drawables;
using Android.Widget;

#endregion

namespace UrlImageViewHelper
{
    public interface IUrlImageViewCallback
    {
        void OnLoaded(ImageView imageView, Drawable loadedDrawable, string url, bool loadedFromCache);
    }
}