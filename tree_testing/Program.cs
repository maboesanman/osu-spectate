using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tree_testing
{
    class test
    {
        public static void Main(string[] args)
        {
            Tree<int> stuff = new Tree<int>();
            var numberList = Enumerable.Range(0, 15).ToList();
            numberList = numberList.OrderBy(a => Guid.NewGuid()).ToList();
            Console.WriteLine("hi");
            for (int i = 0; i < numberList.Count; i++)
            {
                stuff.Add(numberList.ElementAt(i));
            }
            stuff.Display();
            Console.WriteLine();
            for (int i = 0; i < numberList.Count; i++)
            {
                stuff.Remove(numberList.ElementAt(numberList.ElementAt(i)));
                Console.Write(numberList.ElementAt(numberList.ElementAt(i)) + " ");
                stuff.Display();
            }
            Console.ReadKey();

        }
        enum Color
        {
            Red,
            Black
        }
        public class Tree<T> where T : IComparable<T>
        {
            private Node root;
            public int Count;
            public Tree()
            {
                Count = 0;
            }
            public bool isEmpty()
            {
                return root == null;
            }
            public void Add(T value)
            {
                if (root == null)
                {
                    root = new Node(value, this);
                    return;
                }
                root.Add(value);
                Count++;
            }
            public bool Remove(T value)
            {
                if (root == null)
                {
                    return false;
                }
                bool temp = root.Remove(value);
                if (temp)
                {
                    Count--;
                }
                return temp;
            }
            public void Display()
            {
                if (root == null)
                {
                    Console.WriteLine("nothing");
                    return;
                }
                root.display();
                Console.WriteLine();
            }
            public T First()
            {
                if (isEmpty())
                {
                    return default(T);
                }
                return root.Minimum().item;
            }
            public T Last()
            {
                if (isEmpty())
                {
                    return default(T);
                }
                return root.Maximum().item;
            }
            public T Floor(T value)
            {
                if (root == null)
                {
                    Console.WriteLine("none");
                    return default(T);

                }
                Node temp = root.Floor(value);
                if (temp == null)
                {
                    Console.WriteLine("none");
                    return default(T);

                }
                return temp.item;
            }
            public T Ceiling(T value)
            {
                if (root == null)
                {
                    Console.WriteLine("none");
                    return default(T);

                }
                Node temp = root.Ceiling(value);
                if (temp == null)
                {
                    Console.WriteLine("none");
                    return default(T);

                }
                return temp.item;
            }
            public bool Contains(T value)
            {
                if (root == null)
                {
                    return false;
                }
                return root.Contains(value);
            }
            private class Node
            {
                public T item;
                public Node parent;
                public Node left;
                public Node right;
                public Tree<T> container;
                public Node(T i, Tree<T> c)
                {
                    item = i;
                    container = c;
                }
                public void Add(T value)
                {
                    if (value.CompareTo(item) <= 0)
                    {
                        if (left == null)
                        {
                            left = new Node(value, container);
                            left.parent = this;
                            return;
                        }
                        else
                        {
                            left.Add(value);
                            return;
                        }
                    }
                    else
                    {
                        if (right == null)
                        {
                            right = new Node(value, container);
                            right.parent = this;
                            return;
                        }
                        else
                        {
                            right.Add(value);
                            return;
                        }
                    }
                }
                public bool Remove(T value)
                {
                    if (value.CompareTo(item) == 0)
                    {
                        if (left == null && right == null)
                        {
                            if (this == container.root)
                            {
                                container.root = null;
                                return true;
                            }
                            if (this == parent.left)
                            {
                                parent.left = null;
                            }
                            if (this == parent.right)
                            {
                                parent.right = null;
                            }
                        }
                        if (left == null && right != null)
                        {
                            if (this == container.root)
                            {
                                container.root = right;
                                return true;
                            }
                            if (this == parent.left)
                            {
                                parent.left = right;
                            }
                            if (this == parent.right)
                            {
                                parent.right = right;
                            }
                            right.parent = parent;
                        }
                        if (left != null && right == null)
                        {
                            if (this == container.root)
                            {
                                container.root = left;
                                return true;
                            }
                            if (this == parent.left)
                            {
                                parent.left = left;
                            }
                            if (this == parent.right)
                            {
                                parent.right = left;
                            }
                            left.parent = parent;
                        }
                        if (left != null && right != null)
                        {
                            Node temp = right.Minimum();
                            right.Remove(temp.item);
                            item = temp.item;
                        }
                        return true;
                    }
                    if (value.CompareTo(item) < 0)
                    {
                        if (left != null)
                        {
                            return left.Remove(value);
                        }
                        return false;
                    }
                    if (value.CompareTo(item) > 0)
                    {
                        if (right != null)
                        {
                            return right.Remove(value);
                        }
                        return false;
                    }
                    return false;
                }
                public bool Contains(T query)
                {
                    if (query.CompareTo(item) == 0)
                    {
                        return true;
                    }
                    if (query.CompareTo(item) < 0)
                    {
                        if (left != null)
                        {
                            return left.Contains(query);
                        }
                    }
                    if (query.CompareTo(item) > 0)
                    {
                        if (right != null)
                        {
                            return right.Contains(query);
                        }
                    }
                    return false;
                }
                public Node Floor(T query)
                {
                    if (left == null && right == null)
                    {
                        if (query.CompareTo(item) >= 0)
                        {
                            return this;
                        }
                        return null;
                    }
                    if (query.CompareTo(item) == 0)
                    {
                        return this;
                    }
                    if (query.CompareTo(item) <= 0)
                    {
                        if (left == null)
                        {
                            return null;
                        }
                        return left.Floor(query);
                    }
                    if (query.CompareTo(item) >= 0)
                    {
                        if (right == null)
                        {
                            return this;
                        }
                        Node temp = right.Floor(query);
                        if (temp != null)
                        {
                            return temp;
                        }
                        return this;
                    }
                    return null;
                }
                public Node Ceiling(T query)
                {
                    if (left == null && right == null)
                    {
                        if (query.CompareTo(item) <= 0)
                        {
                            return this;
                        }
                        return null;
                    }
                    if (query.CompareTo(item) == 0)
                    {
                        return this;
                    }
                    if (query.CompareTo(item) >= 0)
                    {
                        if (right == null)
                        {
                            return null;
                        }
                        return right.Ceiling(query);
                    }
                    if (query.CompareTo(item) <= 0)
                    {
                        if (left == null)
                        {
                            return this;
                        }
                        Node temp = left.Ceiling(query);
                        if (temp != null)
                        {
                            return temp;
                        }
                        return this;
                    }
                    return null;
                }
                public Node Minimum()
                {
                    if (left == null)
                    {
                        return this;
                    }
                    return left.Minimum();
                }
                public Node Maximum()
                {
                    if (right == null)
                    {
                        return this;
                    }
                    return right.Maximum();
                }
                public void display()
                {

                    if (left != null) { left.display(); }
                    Console.Write("({0}) ", item);
                    if (right != null) { right.display(); }
                }
            }
        }


    }
}
