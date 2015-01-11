#region

using System;
using Quobject.EngineIoClientDotNet.ComponentEmitter;

#endregion

namespace Quobject.SocketIoClientDotNet.Client
{
    public class On
    {
        private On()
        {
        }

        public static IHandle Create(Emitter obj, string ev, IListener fn)
        {
            obj.On(ev, fn);
            return new HandleImpl(obj, ev, fn);
        }

        public class HandleImpl : IHandle
        {
            private readonly string ev;
            private readonly IListener fn;
            private readonly Emitter obj;

            public HandleImpl(Emitter obj, string ev, IListener fn)
            {
                this.obj = obj;
                this.ev = ev;
                this.fn = fn;
            }

            public void Destroy()
            {
                obj.Off(ev, fn);
            }
        }

        public class ActionHandleImpl : IHandle
        {
            private readonly Action fn;

            public ActionHandleImpl(Action fn)
            {
                this.fn = fn;
            }

            public void Destroy()
            {
                fn();
            }
        }

        public interface IHandle
        {
            void Destroy();
        }
    }
}