using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace OGSAndroid
{
    public class SGFTree<T>
    {
        public Node<T> FirstNode;
        public List<Node<T>> Nodes = new List<Node<T>>();

        public SGFTree()
        {
            Nodes = new List<Node<T>>();
        }
                 
        public void PopulateNodesList()
        {
            Nodes = Node<T>.Enumerator(FirstNode).ToList();
        }

    }

    public class Node<T>
    {
        private List<Node<T>> children;
        public SGFTree<T> Parent;
        public T Data { get; set; }

        public bool HasVariation
        {
            get
            {
                return children.Count > 0;
            }
            private set { }
        }

        public Node(T item, SGFTree<T> parent)
        {
            children = new List<Node<T>>();
            Parent = parent;
            Data = item;
        }

        public Node<T> AddChild(T item)
        {
            children.Add(new Node<T>(item, Parent));
            return children.Last();
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        public static IEnumerable<Node<T>> Enumerator(Node<T> node)
        {
            yield return node;

            foreach (var n in node.children) 
            {            
                foreach (var nn in Node<T>.Enumerator(n))
                {
                    yield return nn;
                }
            }
        }
    }
}

