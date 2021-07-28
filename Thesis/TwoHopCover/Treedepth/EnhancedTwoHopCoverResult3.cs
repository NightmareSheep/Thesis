using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using Thesis.Trees;
using System.Linq;

namespace Thesis.TwoHopCover.Treedepth
{
    public class EnhancedTwoHopCoverResult3 : TwoHopCoverResult
    {
        Dictionary<int, List<int>> optimizedSeperators = new Dictionary<int, List<int>>();


        public EnhancedTwoHopCoverResult3(Dictionary<int, TreeNode> treeNodes, Dictionary<int, Dictionary<int, Entry>> index, Dictionary<int, Node> nodes, TreeNode root) : base(treeNodes, index, nodes, root)
        {
            this.nodes = nodes;

            var elimOrder = root.GetEliminationOrder();
            elimOrder.Reverse();
            for (var i = 0; i < elimOrder.Count; i++)
            {
                var treeNode = treeNodes[elimOrder[i]];
                Enhance(treeNode);
            }
        }

        private void Enhance(TreeNode root)
        {
            if (root.Children.Count < 2)
                return;

            var improvedSeperator = new HashSet<int>();

            var subtrees = new List<List<TreeNode>>();
            foreach (var child in root.Children)
                subtrees.Add(child.GetTreeNodesInSubtree());

            var vertexSets = new List<List<int>>();
            foreach (var subtree in subtrees)
                vertexSets.Add(GetNeigbourSet(subtree, root.Depth));

            for (int i = 0; i < vertexSets.Count; i++)
            {
                var leftSubtree = vertexSets[i];
                for (int j = i + 1; j < vertexSets.Count; j++)
                {
                    var rightSubtree = vertexSets[j];
                    foreach (var leftNode in leftSubtree)
                        foreach (var rightNode in rightSubtree)
                        {
                            var pathNode = GetPathNode(leftNode, rightNode);
                            improvedSeperator.Add(pathNode.Id);
                        }
                }
            }

            if (improvedSeperator.Count != root.Depth + 1)
                optimizedSeperators[root.Id] = improvedSeperator.ToList();
        }

        protected override TreeNode GetPathNode(int from, int to)
        {
            var lowestCommonAncestor = GetLowestCommonAncestor(from, to);
            var pathNode = TreeNodes[lowestCommonAncestor];
            var pathLength = int.MaxValue;
            var separator = GetSeparator(lowestCommonAncestor);

            foreach (var separatorNode in separator)
            {
                var subIndex = Index[separatorNode];
                var path1Length = subIndex[from].Length;
                var path2Length = subIndex[to].Length;

                var newPathLength = path1Length + path2Length;
                if (newPathLength < pathLength && path1Length != int.MaxValue && path2Length != int.MaxValue)
                {
                    pathLength = newPathLength;
                    pathNode = TreeNodes[separatorNode];
                }
            }

            return pathNode;
        }

        private List<int> GetSeparator(int lowestCommonAncestor)
        {
            if (optimizedSeperators.TryGetValue(lowestCommonAncestor, out var improvedSeparator))
                return improvedSeparator;

            var separator = new List<int>();

            var current = TreeNodes[lowestCommonAncestor];
            while (current != null)
            {
                separator.Add(current.Id);
                current = current.Parent;
            }

            return separator;
        }

        private List<int> GetNeigbourSet(List<TreeNode> subtree, int lowestCommonAncestor)
        {
            var result = new List<int>();

            foreach (var treeNode in subtree)
            {
                var node = nodes[treeNode.Id];
                foreach (var edge in node.Edges)
                {
                    var neigbour = edge.Other(node);
                    var neighbourTreeNode = TreeNodes[neigbour.Id];
                    if (neighbourTreeNode.Depth <= lowestCommonAncestor)
                    {
                        result.Add(treeNode.Id);
                        break;
                    }
                }
            }
            return result;
        }

        public override int Size { get { return base.Size + optimizedSeperators.Count; } }
    }
}
