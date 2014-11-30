#region

using System;
using Android.Widget;
using Object = Java.Lang.Object;

#endregion

namespace OGSAndroid
{
    public class ListViewInfiniteScroll : Object, AbsListView.IOnScrollListener
    {
        private readonly ListView listView;
        public Action HitBottom;

        public ListViewInfiniteScroll(ListView lv)
        {
            lv.SetOnScrollListener(this);
            listView = lv;
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            if (listView == null || listView.Count == 0)
                return;
            int last = firstVisibleItem + visibleItemCount;
            if (listView.Count > last) return; //Not at bottom.
            if (HitBottom != null)
                HitBottom.Invoke();
        }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {
        }
    }
}