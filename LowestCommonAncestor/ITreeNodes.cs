using System;
using System.Collections.Generic;
using System.Text;

namespace LowestCommonAncestor
{
    public interface ITreeNode<T> where T : ITreeNode<T>
    {
        int Id { get; }

        List<T> Children { get; }
    }
}
