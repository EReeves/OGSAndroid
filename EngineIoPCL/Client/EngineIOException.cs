#region

using System;

#endregion

namespace Quobject.EngineIoClientDotNet.Client
{
    public class EngineIOException : Exception
    {
        public object code;
        public string Transport;

        public EngineIOException(string message)
            : base(message)
        {
        }

        public EngineIOException(Exception cause)
            : base("", cause)
        {
        }

        public EngineIOException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}