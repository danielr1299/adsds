using System;
using System.Text;

namespace DataStuctures
{
    public class DoubleLList<T>
    {
        //Node start;
        //Node end;
        public Node Start { get;private set; }
        public Node End { get; private set; }

        public void AddFirst(T value)
        {
            Node node = new Node(value);
            node.Next = Start;
            if (Start != null)
                Start.Previous = node;
            Start = node;
            if (Start.Next == null)
                End = node;
        }

        public void AddLast(T value)
        {
            if (Start == null)
            {
                AddFirst(value);
                return;
            }

            Node node = new Node(value);
            End.Next = node;
            node.Previous = End;
            End = End.Next;
        }

        public bool RemoveFirst(out T removedValue)
        {
            removedValue = default;
            if (Start == null)
                return false;

            removedValue = Start.Data;
            if (Start.Next != null)
                Start.Next.Previous = null;
            Start = Start.Next;

            if (Start == null)
                End = null;
            return true;
        }

        public bool RemoveLast(out T removedValue)
        {
            removedValue = default;
            if (Start == null) return false;

            if (Start.Next == null)
            {
                return RemoveFirst(out removedValue);
            }

            removedValue = End.Data;
            End = End.Previous;
            End.Next.Previous = null;
            End.Next = null;

            return true;
        }

        public bool AddAt(int position, T value)
        {
            if (position < 0)
                throw new ArgumentOutOfRangeException("position not valid");

            if (position == 0)
            {
                AddFirst(value);
                return true;
            }

            Node node = new Node(value);
            Node tmp = Start;
            for (int i = 0; tmp != null && i < position - 1; i++)
            {
                tmp = tmp.Next;
            }

            if (tmp != null)
            {
                node.Next = tmp.Next;
                node.Previous = tmp;
                tmp.Next = node;
                if (node.Next != null)
                    node.Next.Previous = node;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Node tmp = Start;

            while (tmp != null)
            {
                sb.Append($"{tmp.Data} ");
                tmp = tmp.Next;
            }
            return sb.ToString();
        }
        public class Node
        {
            public T Data { get; private set; }
            public Node Next { get; set; }
            public Node Previous { get; set; }

            public Node(T data)
            {
                Data = data;
                Next = null;
                Previous = null;
            }
        }
    }
}
