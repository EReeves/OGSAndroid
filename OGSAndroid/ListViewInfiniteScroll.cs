using System;
using Android.App;
using Android.Content;

using Android.Views;
using Android.Widget;

namespace OGSAndroid
{
    public class ListViewInfiniteScroll : Java.Lang.Object, AbsListView.IOnScrollListener
    {
        public Action HitBottom;
        private ListView listView;

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
            if (listView.Count <= last)
            {
                if(HitBottom != null)
                    HitBottom.Invoke();
            }
        }

        public void OnScrollStateChanged(AbsListView view, ScrollState scrollState)
        {

        }
    }
}

