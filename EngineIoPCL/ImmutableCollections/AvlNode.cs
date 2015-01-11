#region

using System;
using System.Collections.Generic;

#endregion

namespace Quobject.Collections.Immutable
{
    internal class AvlNode<T>
    {
        public static readonly AvlNode<T> Empty = new NullNode();
        public T Value;

        public AvlNode()
        {
            Right = Empty;
            Left = Empty;
        }

        public AvlNode(T val) : this(val, Empty, Empty)
        {
        }

        private AvlNode(T val, AvlNode<T> lt, AvlNode<T> gt)
        {
            Value = val;
            Left = lt;
            Right = gt;
            Count = 1 + Left.Count + Right.Count;
            Depth = 1 + Math.Max(Left.Depth, Right.Depth);
        }

        public virtual bool IsEmpty
        {
            get { return false; }
        }

        public AvlNode<T> Left { get; private set; }
        public AvlNode<T> Right { get; private set; }
        public int Count { get; private set; }

        private int Balance
        {
            get
            {
                if (IsEmpty)
                {
                    return 0;
                }
                return Left.Depth - Right.Depth;
            }
        }

        private int Depth { get; set; }

        /// <summary>
        ///     Return the subtree with the min value at the root, or Empty if Empty
        /// </summary>
        private AvlNode<T> Min
        {
            get
            {
                if (IsEmpty)
                    return Empty;
                var dict = this;
                var next = dict.Left;
                while (next != Empty)
                {
                    dict = next;
                    next = dict.Left;
                }
                return dict;
            }
        }

        public virtual bool IsMutable
        {
            get { return false; }
        }

        /// <summary>
        ///     Fix the root balance if LTDict and GTDict have good balance
        ///     Used to keep the depth less than 1.44 log_2 N (AVL tree)
        /// </summary>
        private AvlNode<T> FixRootBalance()
        {
            int bal = Balance;
            if (Math.Abs(bal) < 2)
                return this;

            if (bal == 2)
            {
                if (Left.Balance == 1 || Left.Balance == 0)
                {
                    //Easy case:
                    return RotateToGT();
                }
                if (Left.Balance == -1)
                {
                    //Rotate LTDict:
                    var newlt = ToMutableIfNecessary(Left).RotateToLT();
                    var newroot = NewOrMutate(Value, newlt, Right);
                    return newroot.RotateToGT();
                }
                throw new Exception(String.Format("LTDict too unbalanced: {0}", Left.Balance));
            }
            if (bal == -2)
            {
                if (Right.Balance == -1 || Right.Balance == 0)
                {
                    //Easy case:
                    return RotateToLT();
                }
                if (Right.Balance == 1)
                {
                    //Rotate GTDict:
                    var newgt = ToMutableIfNecessary(Right).RotateToGT();
                    var newroot = NewOrMutate(Value, Left, newgt);
                    return newroot.RotateToLT();
                }
                throw new Exception(String.Format("LTDict too unbalanced: {0}", Left.Balance));
            }
            //In this case we can show: |bal| > 2
            //if( Math.Abs(bal) > 2 ) {
            throw new Exception(String.Format("Tree too out of balance: {0}", Balance));
        }

        public AvlNode<T> SearchNode(T value, Comparison<T> comparer)
        {
            var dict = this;
            while (dict != Empty)
            {
                int comp = comparer(dict.Value, value);
                if (comp < 0)
                {
                    dict = dict.Right;
                }
                else if (comp > 0)
                {
                    dict = dict.Left;
                }
                else
                {
                    //Awesome:
                    return dict;
                }
            }
            return Empty;
        }

        /// <summary>
        ///     Return a new tree with the key-value pair inserted
        ///     If the key is already present, it replaces the value
        ///     This operation is O(Log N) where N is the number of keys
        /// </summary>
        public AvlNode<T> InsertIntoNew(int index, T val)
        {
            if (IsEmpty)
                return new AvlNode<T>(val);

            AvlNode<T> newlt = Left;
            AvlNode<T> newgt = Right;

            if (index <= Left.Count)
            {
                newlt = ToMutableIfNecessary(Left).InsertIntoNew(index, val);
            }
            else
            {
                newgt = ToMutableIfNecessary(Right).InsertIntoNew(index - Left.Count - 1, val);
            }

            var newroot = NewOrMutate(Value, newlt, newgt);
            return newroot.FixRootBalance();
        }

        /// <summary>
        ///     Return a new tree with the key-value pair inserted
        ///     If the key is already present, it replaces the value
        ///     This operation is O(Log N) where N is the number of keys
        /// </summary>
        public AvlNode<T> InsertIntoNew(T val, Comparison<T> comparer)
        {
            if (IsEmpty)
                return new AvlNode<T>(val);

            AvlNode<T> newlt = Left;
            AvlNode<T> newgt = Right;

            int comp = comparer(Value, val);
            T newv = Value;
            if (comp < 0)
            {
                //Let the GTDict put it in:
                newgt = ToMutableIfNecessary(Right).InsertIntoNew(val, comparer);
            }
            else if (comp > 0)
            {
                //Let the LTDict put it in:
                newlt = ToMutableIfNecessary(Left).InsertIntoNew(val, comparer);
            }
            else
            {
                //Replace the current value:
                newv = val;
            }
            var newroot = NewOrMutate(newv, newlt, newgt);
            return newroot.FixRootBalance();
        }

        public AvlNode<T> SetItem(int index, T val)
        {
            AvlNode<T> newlt = Left;
            AvlNode<T> newgt = Right;

            if (index < Left.Count)
            {
                newlt = ToMutableIfNecessary(Left).SetItem(index, val);
            }
            else if (index > Left.Count)
            {
                newgt = ToMutableIfNecessary(Right).SetItem(index - Left.Count - 1, val);
            }
            else
            {
                return NewOrMutate(val, newlt, newgt);
            }
            return NewOrMutate(Value, newlt, newgt);
        }

        public AvlNode<T> GetNodeAt(int index)
        {
            if (index < Left.Count)
                return Left.GetNodeAt(index);
            if (index > Left.Count)
                return Right.GetNodeAt(index - Left.Count - 1);
            return this;
        }

        /// <summary>
        ///     Try to remove the key, and return the resulting Dict
        ///     if the key is not found, old_node is Empty, else old_node is the Dict
        ///     with matching Key
        /// </summary>
        public AvlNode<T> RemoveFromNew(int index, out bool found)
        {
            if (IsEmpty)
            {
                found = false;
                return Empty;
            }

            if (index < Left.Count)
            {
                var newlt = ToMutableIfNecessary(Left).RemoveFromNew(index, out found);
                if (!found)
                {
                    //Not found, so nothing changed
                    return this;
                }
                var newroot = NewOrMutate(Value, newlt, Right);
                return newroot.FixRootBalance();
            }

            if (index > Left.Count)
            {
                var newgt = ToMutableIfNecessary(Right).RemoveFromNew(index - Left.Count - 1, out found);
                if (!found)
                {
                    //Not found, so nothing changed
                    return this;
                }
                var newroot = NewOrMutate(Value, Left, newgt);
                return newroot.FixRootBalance();
            }

            //found it
            found = true;
            return RemoveRoot();
        }

        /// <summary>
        ///     Try to remove the key, and return the resulting Dict
        ///     if the key is not found, old_node is Empty, else old_node is the Dict
        ///     with matching Key
        /// </summary>
        public AvlNode<T> RemoveFromNew(T val, Comparison<T> comparer, out bool found)
        {
            if (IsEmpty)
            {
                found = false;
                return Empty;
            }
            int comp = comparer(Value, val);
            if (comp < 0)
            {
                var newgt = ToMutableIfNecessary(Right).RemoveFromNew(val, comparer, out found);
                if (!found)
                {
                    //Not found, so nothing changed
                    return this;
                }
                var newroot = NewOrMutate(Value, Left, newgt);
                return newroot.FixRootBalance();
            }
            if (comp > 0)
            {
                var newlt = ToMutableIfNecessary(Left).RemoveFromNew(val, comparer, out found);
                if (!found)
                {
                    //Not found, so nothing changed
                    return this;
                }
                var newroot = NewOrMutate(Value, newlt, Right);
                return newroot.FixRootBalance();
            }
            //found it
            found = true;
            return RemoveRoot();
        }

        private AvlNode<T> RemoveMax(out AvlNode<T> max)
        {
            if (IsEmpty)
            {
                max = Empty;
                return Empty;
            }
            if (Right.IsEmpty)
            {
                //We are the max:
                max = this;
                return Left;
            }
            //Go down:
            var newgt = ToMutableIfNecessary(Right).RemoveMax(out max);
            var newroot = NewOrMutate(Value, Left, newgt);
            return newroot.FixRootBalance();
        }

        private AvlNode<T> RemoveMin(out AvlNode<T> min)
        {
            if (IsEmpty)
            {
                min = Empty;
                return Empty;
            }
            if (Left.IsEmpty)
            {
                //We are the minimum:
                min = this;
                return Right;
            }
            //Go down:
            var newlt = ToMutableIfNecessary(Left).RemoveMin(out min);
            var newroot = NewOrMutate(Value, newlt, Right);
            return newroot.FixRootBalance();
        }

        /// <summary>
        ///     Return a new dict with the root key-value pair removed
        /// </summary>
        private AvlNode<T> RemoveRoot()
        {
            if (IsEmpty)
            {
                return this;
            }
            if (Left.IsEmpty)
            {
                return Right;
            }
            if (Right.IsEmpty)
            {
                return Left;
            }
            //Neither are empty:
            if (Left.Count < Right.Count)
            {
                //LTDict has fewer, so promote from GTDict to minimize depth
                AvlNode<T> min;
                var newgt = ToMutableIfNecessary(Right).RemoveMin(out min);
                var newroot = NewOrMutate(min.Value, Left, newgt);
                return newroot.FixRootBalance();
            }
            else
            {
                AvlNode<T> max;
                var newlt = ToMutableIfNecessary(Left).RemoveMax(out max);
                var newroot = NewOrMutate(max.Value, newlt, Right);
                return newroot.FixRootBalance();
            }
        }

        /// <summary>
        ///     Move the Root into the GTDict and promote LTDict node up
        ///     If LTDict is empty, this operation returns this
        /// </summary>
        private AvlNode<T> RotateToGT()
        {
            if (Left.IsEmpty || IsEmpty)
            {
                return this;
            }
            var newLeft = ToMutableIfNecessary(Left);
            var lL = newLeft.Left;
            var lR = newLeft.Right;
            var newRight = NewOrMutate(Value, lR, Right);
            return newLeft.NewOrMutate(newLeft.Value, lL, newRight);
        }

        /// <summary>
        ///     Move the Root into the LTDict and promote GTDict node up
        ///     If GTDict is empty, this operation returns this
        /// </summary>
        private AvlNode<T> RotateToLT()
        {
            if (Right.IsEmpty || IsEmpty)
            {
                return this;
            }
            var newRight = ToMutableIfNecessary(Right);
            var rL = newRight.Left;
            var rR = newRight.Right;
            var newLeft = NewOrMutate(Value, Left, rL);
            return newRight.NewOrMutate(newRight.Value, newLeft, rR);
        }

        /// <summary>
        ///     Enumerate from largest to smallest key
        /// </summary>
        public IEnumerator<T> GetEnumerator(bool reverse)
        {
            var to_visit = new Stack<AvlNode<T>>();
            to_visit.Push(this);
            while (to_visit.Count > 0)
            {
                var this_d = to_visit.Pop();
                continue_without_pop:
                if (this_d.IsEmpty)
                {
                    continue;
                }
                if (reverse)
                {
                    if (this_d.Right.IsEmpty)
                    {
                        //This is the next biggest value in the Dict:
                        yield return this_d.Value;
                        this_d = this_d.Left;
                        goto continue_without_pop;
                    }
                    //Break it up
                    to_visit.Push(this_d.Left);
                    to_visit.Push(new AvlNode<T>(this_d.Value));
                    this_d = this_d.Right;
                    goto continue_without_pop;
                }
                if (this_d.Left.IsEmpty)
                {
                    //This is the next biggest value in the Dict:
                    yield return this_d.Value;
                    this_d = this_d.Right;
                    goto continue_without_pop;
                }
                //Break it up
                if (!this_d.Right.IsEmpty)
                    to_visit.Push(this_d.Right);
                to_visit.Push(new AvlNode<T>(this_d.Value));
                this_d = this_d.Left;
                goto continue_without_pop;
            }
        }

        public IEnumerable<T> Enumerate(int index, int count, bool reverse)
        {
            // TODO!
            int i;
            var e = GetEnumerator(reverse);
            if (!reverse)
            {
                i = 0;
                while (e.MoveNext())
                {
                    if (index <= i)
                        yield return e.Current;
                    i++;
                    if (i >= index + count)
                        break;
                }
            }
            else
            {
                i = Count - 1;
                while (e.MoveNext())
                {
                    if (i <= index)
                        yield return e.Current;
                    i--;
                    if (i <= index - count)
                        break;
                }
            }
        }

        public virtual AvlNode<T> ToImmutable()
        {
            return this;
        }

        public virtual AvlNode<T> NewOrMutate(T newValue, AvlNode<T> newLeft, AvlNode<T> newRight)
        {
            return new AvlNode<T>(newValue, newLeft, newRight);
        }

        public virtual AvlNode<T> ToMutable()
        {
            //throw new NotImplementedException ();
            return new MutableAvlNode(Value, Left, Right);
        }

        public virtual AvlNode<T> ToMutableIfNecessary(AvlNode<T> node)
        {
            return node;
        }

        private sealed class NullNode : AvlNode<T>
        {
            public override bool IsEmpty
            {
                get { return true; }
            }

            public override AvlNode<T> NewOrMutate(T newValue, AvlNode<T> newLeft, AvlNode<T> newRight)
            {
                throw new NotSupportedException();
            }

            public override AvlNode<T> ToMutable()
            {
                return this;
            }
        }

        private sealed class MutableAvlNode : AvlNode<T>
        {
            public MutableAvlNode(T val, AvlNode<T> lt, AvlNode<T> gt) : base(val, lt, gt)
            {
            }

            public override bool IsMutable
            {
                get { return true; }
            }

            public override AvlNode<T> ToImmutable()
            {
                return new AvlNode<T>(Value, Left.ToImmutable(), Right.ToImmutable());
            }

            public override AvlNode<T> NewOrMutate(T newValue, AvlNode<T> newLeft, AvlNode<T> newRight)
            {
                Value = newValue;
                Left = newLeft;
                Right = newRight;
                Count = 1 + Left.Count + Right.Count;
                Depth = 1 + Math.Max(Left.Depth, Right.Depth);
                return this;
            }

            public override AvlNode<T> ToMutable()
            {
                return this;
            }

            public override AvlNode<T> ToMutableIfNecessary(AvlNode<T> node)
            {
                return node.ToMutable();
            }
        }
    }
}