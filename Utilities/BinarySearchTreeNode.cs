namespace ADTEfficiencyMeasurements.Utilities;

public class BinarySearchTreeNode<TKey, TValue> where TKey : IComparable<TKey>
{
    public TKey Key { get; internal set; }
    public TValue Data { get; set; }
    public BinarySearchTreeNode<TKey, TValue> Parent { get; set; }
    public List<BinarySearchTreeNode<TKey, TValue>> Children { get; set; }
    public BinarySearchTreeNode<TKey, TValue> Left {
        get { return (BinarySearchTreeNode<TKey, TValue>)Children[0]; }
        set { Children[0] = value; }
    }
    public BinarySearchTreeNode<TKey, TValue> Right {
        get { return (BinarySearchTreeNode<TKey, TValue>)Children[1]; }
        set { Children[1] = value; }
    }

    public BinarySearchTreeNode()
    {
        Children = new List<BinarySearchTreeNode<TKey, TValue>>() { null, null };
    }

    public BinarySearchTreeNode(TKey key, TValue data, BinarySearchTreeNode<TKey, TValue> parent)
        : this()
    {
        Key = key;
        Data = data;
        Parent = parent;
    }

    public int GetHeight()
    {
        int height = 1;
        BinarySearchTreeNode<TKey, TValue> current = this;
        while (current.Parent != null) {
            height++;
            current = current.Parent;
        }
        return height;
    }

}