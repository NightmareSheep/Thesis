using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using LowestCommonAncestor;

namespace Thesis.Trees
{
    public class TreeNode : ITreeNode<TreeNode>
    {
        public int Id { get; set; }
        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();
        public int Depth { get; set; }
    }
}
