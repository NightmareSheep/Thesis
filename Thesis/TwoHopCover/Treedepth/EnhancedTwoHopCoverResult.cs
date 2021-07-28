using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using Thesis.Trees;

namespace Thesis.TwoHopCover.Treedepth
{
    public class EnhancedTwoHopCoverResult : TwoHopCoverResult
    {
        private readonly Dictionary<int, HashSet<int>> optimizedSeperators = new Dictionary<int, HashSet<int>>();

        public EnhancedTwoHopCoverResult(Dictionary<int, TreeNode> treeNodes, Dictionary<int, Dictionary<int, Entry>> index, Dictionary<int, Node> nodes, TreeNode root) : base(treeNodes, index, nodes, root)
        {
            this.nodes = nodes;
            Enhance(root);
        }

        private HashSet<int> Enhance(TreeNode treenode)
        {
            var seperator = new HashSet<int>();
            List<HashSet<int>> childSeperators = new List<HashSet<int>>();


            var indexOfBiggestSet = 0;
            var sizeOfBiggestSet = 0;
            for (int i = 0; i < treenode.Children.Count; i++)
            {
                var child = treenode.Children[i];
                var childSeperator = Enhance(child);
                
                var size = childSeperator.Count;
                if (size > sizeOfBiggestSet)
                {
                    sizeOfBiggestSet = size;
                    indexOfBiggestSet = i;
                }
                childSeperators.Add(childSeperator);
            }

            foreach (var childSeperator in childSeperators)
                seperator.UnionWith(childSeperator);

            if (treenode.Children.Count > 1)
            {
                childSeperators.RemoveAt(indexOfBiggestSet);
                var enhancedSeperator = new HashSet<int>();
                foreach (var childSeperator in childSeperators)
                    enhancedSeperator.UnionWith(childSeperator);

                if (enhancedSeperator.Count < treenode.Depth + 1)
                    optimizedSeperators[treenode.Id] = enhancedSeperator;
            }

            var node = nodes[treenode.Id];
            foreach(var edge in node.Edges)
            {
                var neighbour = edge.Other(node);
                var neighbourTreeNode = TreeNodes[neighbour.Id];
                if (neighbourTreeNode.Depth < treenode.Depth)
                    seperator.Add(neighbour.Id);
            }

            seperator.Remove(treenode.Id);
            return seperator;
        }

        public override int[] Query(int from, int to)
        {
            var lowestCommonAncestor = GetLowestCommonAncestor(from, to);

            var current = TreeNodes[lowestCommonAncestor];
            var pathNode = current;
            var pathLength = int.MaxValue;

            if (!optimizedSeperators.ContainsKey(lowestCommonAncestor))
                return base.Query(from, to);


            var seperator = optimizedSeperators[lowestCommonAncestor];
            foreach (var nodeId in seperator)
            {
                current = TreeNodes[nodeId];
                var subIndex = Index[current.Id];
                var path1Length = subIndex[from].Length;
                var path2Length = subIndex[to].Length;

                var newPathLength = path1Length + path2Length;
                if (newPathLength < pathLength && path1Length != int.MaxValue && path2Length != int.MaxValue)
                {
                    pathLength = newPathLength;
                    pathNode = current;
                }
            }

            if (pathLength == int.MaxValue)
                return base.Query(from, to);

            var path1 = GetPath(pathNode.Id, from);
            path1.Reverse();
            var path2 = GetPath(pathNode.Id, to);


            var path = new List<int>();
            path.AddRange(path1);
            path.Add(pathNode.Id);
            path.AddRange(path2);

            return path.ToArray();
        }

    }
}
