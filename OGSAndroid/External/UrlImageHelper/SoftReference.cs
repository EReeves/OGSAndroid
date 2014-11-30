namespace UrlImageViewHelper
{
    //class SoftReference : WeakReference { public SoftReference(object target) : base(target) { NonWebCache[Guid.NewGuid().ToString()] = target; } }

    // Switched to trusting the GC for mono, so using WeakReference instead
    // public class SoftReference<T> : WeakReference /* where T : Object */ { 
    //     public SoftReference(T target) : base(target)
    //     {
    //     }
    public class SoftReference<T>
    {
        private readonly T _target;

        public SoftReference(T target)
        {
            _target = target;
        }

        public T Get()
        {
            return _target;

            //Android.Util.Log.Debug(UrlImageViewHelper.LOGTAG, "SoftReference OK? " + base.IsAlive.ToString());

            //if (!base.IsAlive)
            //{
            //	Android.Util.Log.Debug(UrlImageViewHelper.LOGTAG, "Lost SoftReference");
            //	return default(T);

            //	//throw new Exception("Lost SoftReference");
            //}
            //return (T)base.Target;
        }
    }
}