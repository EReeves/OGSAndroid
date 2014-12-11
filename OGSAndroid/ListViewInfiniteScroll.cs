#region

using System;
using Android.Widget;
using Object = Java.Lang.Object;

#endregion

namespace OGSAndroid
{
    public class ListViewInfiniteScroll : Object, AbsListView.IOnScrollListener
    {
        public Action HitBottom;
        private readonly ListView listView;

        public ListViewInfiniteScroll(ListView lv)
        {
            lv.SetOnScrollListener(this);
            listView = lv;
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            if (listView == null || listView.Count == 0)
                return;
            var last = firstVisibleItem + visibleItemCount;
            if (listView.Count > last) return; //Not at bottom.
            if (HitBottom != null)
                HitBottom.Invoke();
        }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
        }
    }
}