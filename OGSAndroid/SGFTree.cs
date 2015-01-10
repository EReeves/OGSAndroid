#region

using System.Collections.Generic;
using System.Linq;

#endregion

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

        public Node<T> LastNode()
        {
            if (FirstNode == null)
                return null;

            Node<T> node = FirstNode;
            while (node.HasChild)
            {
                node = node.Children.Last();
            }
            return node;
        }

        public void AddToEnd(T s)
        {
            var n = LastNode();
            if (n == null)
                FirstNode = new Node<T>(s, this);
            else
                n.AddChild(s);

            Nodes.Add(LastNode());
        }
    }

    public class Node<T>
    {
        public SGFTree<T> Parent;
        public readonly List<Node<T>> Children;

        public Node(T item, SGFTree<T> parent)
        {
            Children = new List<Node<T>>();
            Parent = parent;
            Data = item;
        }

        public T Data { get; set; }

        public bool HasChild
        {
            get { return Children.Count > 0; }
        }

        public Node<T> AddChild(T item)
        {
           Children.Add(new Node<T>(item, Parent));
            return Children.Last();
        }

        public override string ToString()
        {
            return Data.ToString();
        }

        public static IEnumerable<Node<T>> Enumerator(Node<T> node)
        {
            yield return node;

            foreach (var nn in node.Children.SelectMany(Enumerator))
            {
                yield return nn;
            }
        }


    }
}