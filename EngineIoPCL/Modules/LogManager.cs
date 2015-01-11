#region

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

#endregion

namespace Quobject.EngineIoClientDotNet.Modules
{
    public class LogManager
    {
        private const string myFileName = "XunitTrace.txt";
        private static readonly LogManager EmptyLogger = new LogManager(null);
        private static StreamWriter file;
        public static bool Enabled = false;
        private readonly string MyType;

        public LogManager(string type)
        {
            MyType = type;
        }

        [Conditional("DEBUG")]
        public void Info(string msg)
        {
            //Trace.WriteLine(string.Format("{0} [{3}] {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), MyType, msg, System.Threading.Thread.CurrentThread.ManagedThreadId));
            if (!Enabled)
            {
                return;
            }


            msg = Global.StripInvalidUnicodeCharacters(msg);
            var msg1 = string.Format("{0} [{3}] {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), MyType,
                msg);
            Debug.WriteLine(msg1);
        }

        [Conditional("DEBUG")]
        public void Error(string p, Exception exception)
        {
            Info(string.Format("ERROR {0} {1} {2}", p, exception.Message, exception.StackTrace));
            if (exception.InnerException != null)
            {
                Info(string.Format("ERROR exception.InnerException {0} {1} {2}", p, exception.InnerException.Message,
                    exception.InnerException.StackTrace));
            }
        }

        [Conditional("DEBUG")]
        internal void Error(Exception e)
        {
            Error("", e);
        }

        #region Statics

        public static void SetupLogManager()
        {
        }

        public static LogManager GetLogger(string type)
        {
            var result = new LogManager(type);
            return result;
        }

        public static LogManager GetLogger(Type type)
        {
            return GetLogger(type.ToString());
        }

        public static LogManager GetLogger(MethodBase methodBase)
        {
#if DEBUG
            var type = methodBase.DeclaringType == null ? "" : methodBase.DeclaringType.ToString();
            var type1 = string.Format("{0}#{1}", type, methodBase.Name);
            return GetLogger(type1);
#else
            return EmptyLogger;
#endif
        }

        #endregion
    }
}