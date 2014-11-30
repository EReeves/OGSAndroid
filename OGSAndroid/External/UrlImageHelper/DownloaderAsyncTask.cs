#region

using System;
using Android.OS;
using Object = Java.Lang.Object;

#endregion

namespace UrlImageViewHelper
{
    public class AnonymousAsyncTask<TParam, TProgress, TResult> : AsyncTask<TParam, TProgress, TResult>
    {
        public Action<TResult> PostExecuteAction;
        public Func<TParam[], TResult> RunInBackgroundFunc;

        public AnonymousAsyncTask(Func<TParam[], TResult> runInBackgroundFunc, Action<TResult> postExecuteAction)
        {
            RunInBackgroundFunc = runInBackgroundFunc;
            PostExecuteAction = postExecuteAction;
        }

        protected override TResult RunInBackground(params TParam[] @params)
        {
            return RunInBackgroundFunc(@params);
        }

        protected override Object DoInBackground(params Object[] native_parms)
        {
            return base.DoInBackground(native_parms);
        }

        protected override void OnPostExecute(TResult result)
        {
            PostExecuteAction(result);
        }
    }
}