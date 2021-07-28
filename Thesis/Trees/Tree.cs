using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thesis.Trees
{
    public class Tree
    {
        public int Depth { get; set; }
        public TreeNode Root { get; set; }
        public TreeNode[] Nodes { get; set; }

        public Tree() { }

        public Tree(int numberOfNodes, Tuple<int, int>[] ParentChildPairs)
        {
            Nodes = new TreeNode[numberOfNodes];
            for (int i = 0; i < numberOfNodes; i++)
                Nodes[i] = new TreeNode() { Id = i };

            foreach (var pair in ParentChildPairs)
            {
                var parentIndex = pair.Item1;
                var childIndex = pair.Item2;

                Nodes[parentIndex].Children.Add(Nodes[childIndex]);
                Nodes[childIndex].Parent = Nodes[parentIndex];
            }

            Root = Nodes.First(n => n.Parent == null);

            SetDepth(0, Root);
        }

        private void SetDepth(int depth, TreeNode node)
        {
            node.Depth = depth;
            Depth = Math.Max(Depth, depth);
            foreach (var child in node.Children)
                SetDepth(depth + 1, child);
        }
    }
}
