#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace Quobject.Collections.Immutable
{
    public class ImmutableStack<T> : IImmutableStack<T>
    {
        private readonly T head;
        private readonly ImmutableStack<T> tail;

        internal ImmutableStack()
        {
        }

        private ImmutableStack(T head, ImmutableStack<T> tail)
        {
            this.head = head;
            this.tail = tail;
        }

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IImmutableStack implementation

        internal static readonly ImmutableStack<T> Empty = new ImmutableStack<T>();

        public bool IsEmpty
        {
            get { return tail == null; }
        }

        public ImmutableStack<T> Clear()
        {
            return Empty;
        }

        IImmutableStack<T> IImmutableStack<T>.Clear()
        {
            return Empty;
        }

        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Stack is empty.");
            return head;
        }

        public ImmutableStack<T> Pop()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Stack is empty.");
            return tail;
        }

        public ImmutableStack<T> Pop(out T value)
        {
            value = Peek();
            return Pop();
        }

        IImmutableStack<T> IImmutableStack<T>.Pop()
        {
            return Pop();
        }

        public ImmutableStack<T> Push(T value)
        {
            return new ImmutableStack<T>(value, this);
        }

        IImmutableStack<T> IImmutableStack<T>.Push(T value)
        {
            return Push(value);
        }

        #endregion

        #region IEnumerable<T> implementation

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        private struct Enumerator : IEnumerator<T>
        {
            private IImmutableStack<T> current;
            private readonly ImmutableStack<T> start;

            public Enumerator(ImmutableStack<T> stack)
            {
                start = stack;
                current = null;
            }

            #region IDisposable implementation

            void IDisposable.Dispose()
            {
            }

            #endregion

            #region IEnumerator implementation

            public T Current
            {
                get { return current != null ? current.Peek() : default(T); }
            }

            #endregion

            #region IEnumerator implementation

            bool IEnumerator.MoveNext()
            {
                if (current == null)
                {
                    current = start;
                }
                else if (!current.IsEmpty)
                {
                    current = current.Pop();
                }

                return !current.IsEmpty;
            }

            void IEnumerator.Reset()
            {
                current = null;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            #endregion
        }

        #endregion
    }

    public static class ImmutableStack
    {
        public static ImmutableStack<T> Create<T>()
        {
            return ImmutableStack<T>.Empty;
        }

        public static ImmutableStack<T> Create<T>(T item)
        {
            return Create<T>().Push(item);
        }

        public static ImmutableStack<T> Create<T>(IEnumerable<T> items)
        {
            var result = ImmutableStack<T>.Empty;
            foreach (var item in items)
                result = result.Push(item);
            return result;
        }

        public static ImmutableStack<T> Create<T>(params T[] items)
        {
            return Create((IEnumerable<T>) items);
        }

        public static IImmutableStack<T> Pop<T>(this IImmutableStack<T> stack, out T value)
        {
            if (stack == null)
                throw new ArgumentNullException("stack");
            value = stack.Peek();
            return stack.Pop();
        }
    }
}