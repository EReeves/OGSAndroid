using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace OGSAndroid
{
    public class SGFTree<T> : IEnumerable<Node<T>>
    {
        private List<Node<T>> nodes;

        public SGFTree()
        {
            nodes = new List<Node<T>>();
        }

        public void AddItem(T item)
        {
            nodes.Add(new Node<T>(item));
        }

        public IEnumerator<Node<T>> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class Node<T> : IEnumerable<Node<T>>
    {
        private List<Node<T>> children;
        public T Data { get; set; }

        public bool HasVariation
        {
            get
            {
                return children.Count > 0;
            }
            private set { }
        }

        public Node(T item)
        {
            children = new List<Node<T>>();
            Data = item;
        }

        public Node<T> AddChild(T item)
        {
            children.Add(new Node<T>(item));
            return children.Last();
        }

        public IEnumerator<Node<T>> GetEnumerator()
        { 
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            string str = "";
            foreach (var n in children) 
            {
                str += n.Data + "\n";
                str += n.ToString() + "\n";
            }

            return str;
        }
    }
}

