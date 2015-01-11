#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace Quobject.Collections.Immutable
{
    public class ImmutableQueue<T> : IImmutableQueue<T>
    {
        private readonly ImmutableStack<T> backStack;
        private readonly ImmutableStack<T> frontStack;

        internal ImmutableQueue()
        {
            frontStack = backStack = ImmutableStack<T>.Empty;
        }

        private ImmutableQueue(ImmutableStack<T> frontStack, ImmutableStack<T> backStack)
        {
            if (frontStack == null)
                throw new ArgumentNullException("frontStack");
            if (backStack == null)
                throw new ArgumentNullException("backStack");

            this.frontStack = frontStack;
            this.backStack = backStack;
        }

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IImmutableQueue implementation

        internal static readonly ImmutableQueue<T> Empty = new ImmutableQueue<T>(ImmutableStack<T>.Empty,
            ImmutableStack<T>.Empty);

        public bool IsEmpty
        {
            get { return frontStack.IsEmpty && backStack.IsEmpty; }
        }

        public ImmutableQueue<T> Clear()
        {
            return Empty;
        }

        IImmutableQueue<T> IImmutableQueue<T>.Clear()
        {
            return Empty;
        }

        public ImmutableQueue<T> Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Queue is empty.");
            var stack = frontStack.Pop();
            if (!stack.IsEmpty)
                return new ImmutableQueue<T>(stack, backStack);
            return new ImmutableQueue<T>(Reverse(backStack), ImmutableStack<T>.Empty);
        }

        public ImmutableQueue<T> Dequeue(out T value)
        {
            value = Peek();
            return Dequeue();
        }

        IImmutableQueue<T> IImmutableQueue<T>.Dequeue()
        {
            return Dequeue();
        }

        private static ImmutableStack<T> Reverse(IImmutableStack<T> stack)
        {
            var result = ImmutableStack<T>.Empty;
            var cur = stack;
            while (!cur.IsEmpty)
            {
                result = result.Push(cur.Peek());
                cur = cur.Pop();
            }
            return result;
        }

        public ImmutableQueue<T> Enqueue(T value)
        {
            if (IsEmpty)
                return new ImmutableQueue<T>(ImmutableStack<T>.Empty.Push(value), ImmutableStack<T>.Empty);

            return new ImmutableQueue<T>(frontStack, backStack.Push(value));
        }

        IImmutableQueue<T> IImmutableQueue<T>.Enqueue(T value)
        {
            return Enqueue(value);
        }

        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Queue is empty.");
            return frontStack.Peek();
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        private struct Enumerator : IEnumerator<T>
        {
            private IImmutableStack<T> backStack;
            private IImmutableStack<T> frontStack;
            private readonly ImmutableQueue<T> start;

            public Enumerator(ImmutableQueue<T> stack)
            {
                start = stack;
                frontStack = null;
                backStack = null;
            }

            #region IDisposable implementation

            void IDisposable.Dispose()
            {
            }

            #endregion

            #region IEnumerator implementation

            public T Current
            {
                get
                {
                    if (frontStack == null)
                        return default(T);
                    if (frontStack.IsEmpty && backStack.IsEmpty)
                        throw new InvalidOperationException();
                    return !frontStack.IsEmpty ? frontStack.Peek() : backStack.Peek();
                }
            }

            #endregion

            #region IEnumerator implementation

            bool IEnumerator.MoveNext()
            {
                if (frontStack == null)
                {
                    frontStack = start.frontStack;
                    backStack = Reverse(start.backStack);
                }
                else if (!frontStack.IsEmpty)
                {
                    frontStack = frontStack.Pop();
                }
                else if (!backStack.IsEmpty)
                {
                    backStack = backStack.Pop();
                }
                return !(frontStack.IsEmpty && backStack.IsEmpty);
            }

            void IEnumerator.Reset()
            {
                frontStack = null;
                backStack = null;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            #endregion
        }

        #endregion
    }

    public static class ImmutableQueue
    {
        public static ImmutableQueue<T> Create<T>()
        {
            return ImmutableQueue<T>.Empty;
        }

        public static ImmutableQueue<T> Create<T>(T item)
        {
            return Create<T>().Enqueue(item);
        }

        public static ImmutableQueue<T> Create<T>(IEnumerable<T> items)
        {
            var result = ImmutableQueue<T>.Empty;
            foreach (var item in items)
                result = result.Enqueue(item);
            return result;
        }

        public static ImmutableQueue<T> Create<T>(params T[] items)
        {
            return Create((IEnumerable<T>) items);
        }

        public static IImmutableQueue<T> Dequeue<T>(this IImmutableQueue<T> queue, out T value)
        {
            if (queue == null)
                throw new ArgumentNullException("queue");
            value = queue.Peek();
            return queue.Dequeue();
        }
    }
}