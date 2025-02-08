using System.Collections;

namespace AbstractDataTypeComparison.Utilities;

public class BinarySearchTree<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
{
    public int Count { get; private set; }

    public BinarySearchTreeNode<TKey, TValue> Root;

    public TValue this[TKey key]
    {
        get { return Get(key); }
        set { Update(key, value); }
    }

    public void Add(TKey key, TValue data)
    {
        BinarySearchTreeNode<TKey, TValue> parent = GetParentForNewNode(key);
        BinarySearchTreeNode<TKey, TValue> node =
            new BinarySearchTreeNode<TKey, TValue>(key, data, parent);
        if (parent == null)
        {
            Root = node;
        }
        else if (key.CompareTo(parent.Key) < 0)
        {
            parent.Left = node;
        }
        else
        {
            parent.Right = node;
        }

        Count++;
    }

    public bool Contains(TKey key)
    {
        BinarySearchTreeNode<TKey, TValue> node = Root;
        while (node != null)
        {
            int result = key.CompareTo(node.Key);
            if (result == 0)
            {
                return true;
            }
            else if (result < 0)
            {
                node = node.Left;
            }
            else
            {
                node = node.Right;
            }
        }

        return false;
    }

    public TValue Get(TKey key)
    {
        BinarySearchTreeNode<TKey, TValue> node = GetNode(key);
        if (node != null)
        {
            return node.Data;
        }
        else
        {
            throw new ApplicationException($"There is no node with key {key}.");
        }
    }

    public void Remove(TKey key)
    {
        Remove(Root, key);
    }

    public void Update(TKey key, TValue value)
    {
        BinarySearchTreeNode<TKey, TValue> node = GetNode(key);
        node.Data = value;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    /*
            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {

                KeyValuePair<TKey, TValue> current =
                foreach (TKey key in ) {
                    return new Enumerator(this);
                }
            }
    */
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    private BinarySearchTreeNode<TKey, TValue> FindMinimumInSubtree(BinarySearchTreeNode<TKey, TValue> node)
    {
        while (node.Left != null)
        {
            node = node.Left;
        }

        return node;
    }

    private BinarySearchTreeNode<TKey, TValue> GetNode(TKey key)
    {
        BinarySearchTreeNode<TKey, TValue> current = Root;

        while (current != null)
        {
            int result = key.CompareTo(current.Key);
            if (result == 0)
            {
                return current;
            }
            else if (result < 0)
            {
                current = current.Left;
            }
            else
            {
                current = current.Right;
            }
        }

        return null;
    }

    private BinarySearchTreeNode<TKey, TValue> GetParentForNewNode(TKey key)
    {
        BinarySearchTreeNode<TKey, TValue> current = Root;
        BinarySearchTreeNode<TKey, TValue> parent = null;

        while (current != null)
        {
            parent = current;
            int result = key.CompareTo(current.Key);
            if (result == 0)
            {
                throw new ArgumentException($"The node {key} already exists.");
            }
            else if (result < 0)
            {
                current = current.Left;
            }
            else
            {
                current = current.Right;
            }
        }

        return parent;
    }

    public void Remove(BinarySearchTreeNode<TKey, TValue> node, TKey key)
    {
        if (node == null)
        {
            throw new ArgumentException($"The node {key} does not exists.");
        }
        else if (key.CompareTo(node.Key) < 0)
        {
            Remove(node.Left, key);
        }
        else if (key.CompareTo(node.Key) > 0)
        {
            Remove(node.Left, key);
        }
        else
        {
            if (node.Left == null && node.Right == null)
            {
                ReplaceInParent(node, null);
                Count--;
            }
            else if (node.Right == null)
            {
                ReplaceInParent(node, node.Left);
                Count--;
            }
            else if (node.Left == null)
            {
                ReplaceInParent(node, node.Right);
                Count--;
            }
            else
            {
                BinarySearchTreeNode<TKey, TValue> successor =
                    FindMinimumInSubtree(node.Right);
                node.Data = successor.Data;
                Remove(successor, successor.Key);
            }
        }
    }

    private void ReplaceInParent(BinarySearchTreeNode<TKey, TValue> node, BinarySearchTreeNode<TKey, TValue> newNode)
    {
        if (node.Parent != null)
        {
            if (node.Parent.Left == node)
            {
                node.Parent.Left = newNode;
            }
            else
            {
                node.Parent.Right = newNode;
            }
        }
        else
        {
            Root = newNode;
        }

        if (newNode != null)
        {
            newNode.Parent = node.Parent;
        }
    }

    private class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private List<BinarySearchTreeNode<TKey, TValue>> allNodes = new List<BinarySearchTreeNode<TKey, TValue>>();
        private List<BinarySearchTreeNode<TKey, TValue>>.Enumerator listEnumerator;

        public KeyValuePair<TKey, TValue> Current
        {
            get { return new KeyValuePair<TKey, TValue>(listEnumerator.Current.Key, listEnumerator.Current.Data); }
            set
            {
                listEnumerator.Current.Key = value.Key;
                listEnumerator.Current.Data = value.Value;
            }
        }

        object IEnumerator.Current
        {
            get { return listEnumerator.Current.Data; }
        }

        internal Enumerator(BinarySearchTree<TKey, TValue> tree)
        {
            MakeSortedList(tree.Root);
            listEnumerator = allNodes.GetEnumerator();
        }

        public bool MoveNext()
        {
            return listEnumerator.MoveNext();
        }

        public void Reset()
        {
            listEnumerator = allNodes.GetEnumerator();
        }

        void IDisposable.Dispose()
        {
            allNodes = null;
        }

        private void MakeSortedList(BinarySearchTreeNode<TKey, TValue> current)
        {
            if (current != null)
            {
                MakeSortedList(current.Left);
                allNodes.Add(current);
                MakeSortedList(current.Right);
            }
        }
    }
}