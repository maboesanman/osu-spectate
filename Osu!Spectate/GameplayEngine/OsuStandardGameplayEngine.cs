using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OsuSpectate.Beatmap;
using ReplayAPI;

namespace OsuSpectate.GameplayEngine
{
    public class OsuStandardGameplayEngine
    {
        Tree<GameplayEvent> eventList;
        Tree<OsuStandardGameplayFrame> gameplayFrames;
        Tree<GameplayObject> objectList;
        List<RenderObject> renderList;
        OsuStandardBeatmap beatmap;
        Mods activeMods;

        public OsuStandardGameplayEngine(OsuStandardBeatmap b, Mods m)
        {
            beatmap = b;
            activeMods = m;
            eventList = new Tree<GameplayEvent>();
            gameplayFrames = new Tree<OsuStandardGameplayFrame>();
            gameplayFrames.Add(new OsuStandardGameplayFrame(TimeSpan.Zero));
            objectList = new Tree<GameplayObject>();
            renderList = new List<RenderObject>();
            var numberList = Enumerable.Range(0, beatmap.GetHitObjectCount()).ToList();
            numberList = numberList.OrderBy(a => Guid.NewGuid()).ToList();
            for (int i = 0; i < beatmap.GetHitObjectCount(); i++)
            {
                OsuStandardHitObject ho = beatmap.GetHitObject(numberList[i], activeMods);
                switch (ho.getType())
                {
                    case ("hitcircle"):
                        new GameplayHitCircle((OsuStandardHitCircle)ho, this, objectList, renderList, eventList);
                        break;
                    case ("slider"):
                        //beatmap.GenerateSliderTexture((OsuStandardSlider)ho,getMods());
                        new RenderSliderBeginEvent((OsuStandardSlider)ho, eventList, renderList, this);
                        break;

                }
            }
        }
        public void HandleUntil(TimeSpan time)
        { 
            bool x = true;
            while (!eventList.isEmpty() && x)
            {
                GameplayEvent first = eventList.First();
                if (first.getTime().CompareTo(time) < 0)
                {
                    if (!first.isHandled())
                    {
                        first.handle();
                    }
                    eventList.Remove(first);
                }
                else
                {
                    x = false;
                }
            }
        }
    
        public void AddClickEvent(ReplayAPI.ReplayFrame frame)
        {
            new ReplayClickEvent(frame, eventList, objectList, renderList, this);
        }
        public OsuStandardGameplayFrame getGameplayFrame(TimeSpan t)
        {
            return gameplayFrames.Floor(new OsuStandardGameplayFrame(t));
        }
        public OsuStandardGameplayFrame getCurrentGameplayFrame()
        {
            return gameplayFrames.Last();
        }
        public TimeSpan GetOD300Milliseconds()
        {
            return beatmap.GetOD300Milliseconds(activeMods);
        }
        public TimeSpan GetOD100Milliseconds()
        {
            return beatmap.GetOD100Milliseconds(activeMods);
        }
        public TimeSpan GetOD50Milliseconds()
        {
            return beatmap.GetOD50Milliseconds(activeMods);
        }
        public TimeSpan GetARMilliseconds()
        {
            return beatmap.GetARMilliseconds(activeMods);
        }
        public float GetCSRadius()
        {
            return beatmap.GetCSRadius(activeMods);
        }
        public Mods getMods()
        {
            return activeMods;
        }
        public List<RenderObject> getRenderList()
        {
            return renderList;
        }
        public OsuStandardBeatmap getBeatmap()
        {
            return beatmap;
        }


    }
    public class Tree<T> : IEnumerable<T> where T : IComparable<T>
    {
        private Node root;
        private Node head;
        private Node tail;
        public int Count;
        
        public Tree()
        {
            Count = 0;
            head = new Node(default(T), this);
            tail = new Node(default(T), this);
            head.next = tail;
            tail.previous = head;
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
                root.previous = head;
                head.next = root;
                root.next = tail;
                tail.previous = root;
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
            Node temp = root.Remove(value);
            if (temp != null)
            {
                Count--;
                temp.previous.next = temp.next;
                temp.next.previous = temp.previous;
                return true;
            }
            return false;
        }
        override public string ToString()
        {
            Node temp = head.next;
            string s = "";
            while (temp != tail)
            {
                s += "(" + temp.item.ToString() + ") ";
                temp = temp.next;
            }
            return s;
        }
        /*
        public void Display()
        {
            if(root!=null)
            {
                root.display();
            }Console.WriteLine();
        }
        */
        public T First()
        {
            if (isEmpty())
            {
                return default(T);
            }
            return head.next.item;
        }
        public T Last()
        {
            if (isEmpty())
            {
                return default(T);
            }
            return tail.previous.item;
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
        public IEnumerator<T> GetEnumerator()
        {
            Node temp = head.next;
            while (temp != tail)
            {
                yield return temp.item;
                temp = temp.next;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            Node temp = head.next;
            while (temp != tail)
            {
                yield return temp.item;
                temp = temp.next;
            }
        }
        public IEnumerator<T> GetReverseEnumerator()
        {
            return new ReverseList(this);
        }


        private class Node
        {
            public T item;
            public Node parent;
            public Node left;
            public Node right;
            public Node next;
            public Node previous;
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
                        left.previous = previous;
                        left.next = this;
                        previous.next = left;
                        previous = left;
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
                        right.previous = this;
                        right.next = next;
                        next.previous = right;
                        next = right;
                        return;
                    }
                    else
                    {
                        right.Add(value);
                        return;
                    }
                }
            }
            public Node Remove(T value)
            {
                if (value.CompareTo(item) == 0)
                {
                    if (left == null && right == null)
                    {
                        if (this == container.root)
                        {
                            container.root = null;
                            return this;
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
                    else
                    if (left == null && right != null)
                    {
                        if (this == container.root)
                        {
                            container.root = right;
                            return this;
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
                    else
                    if (left != null && right == null)
                    {
                        if (this == container.root)
                        {
                            container.root = left;
                            return this;
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
                    else
                    if (left != null && right != null)
                    {
                        Node temp = right.Minimum();
                        item = temp.item;
                        return temp.Remove(temp.item);
                    }
                    return this;
                }
                if (value.CompareTo(item) < 0)
                {
                    if (left != null)
                    {
                        return left.Remove(value);
                    }
                    return null;
                }
                if (value.CompareTo(item) > 0)
                {
                    if (right != null)
                    {
                        return right.Remove(value);
                    }
                    return null;
                }
                return null;
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
        private class ReverseList : IEnumerator<T>
        {
            private Node head;
            private Node tail;
            private Node current;
            public ReverseList(Tree<T> tree)
            {
                head = tree.head;
                tail = tree.tail;
                current = head.next;
            }

            public T Current
            {
                get
                {
                    return current.item;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return current.item;
                }
            }

            public void Dispose()
            {
                
            }

            public IEnumerator<T> GetEnumerator()
            {
                Node temp = tail.previous;
                while (temp != head)
                {
                    yield return temp.item;
                    temp = temp.previous;
                }
            }

            public bool MoveNext()
            {
                if (current!= head)
                {
                    current = current.previous;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                current = tail.previous;
            }
        }
    }
}
