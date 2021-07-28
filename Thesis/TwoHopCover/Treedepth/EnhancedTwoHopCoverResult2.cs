using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using Thesis.Trees;

namespace Thesis.TwoHopCover.Treedepth
{
    public class EnhancedTwoHopCoverResult2 : TwoHopCoverResult
    {
        Dictionary<int, int> optimizedEnd = new Dictionary<int, int>();


        public EnhancedTwoHopCoverResult2(Dictionary<int, TreeNode> treeNodes, Dictionary<int, Dictionary<int, Entry>> index, Dictionary<int, Node> nodes, TreeNode root) : base(treeNodes, index, nodes, root)
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

            var depth = int.MaxValue;

            var subtrees = new List<List<TreeNode>>();
            foreach (var child in root.Children)
                subtrees.Add(child.GetTreeNodesInSubtree());

            for (int i = 0; i < subtrees.Count; i++)
            {
                var leftSubtree = subtrees[i];
                for (int j = i + 1; j < subtrees.Count; j++)
                {
                    var rightSubtree = subtrees[j];
                    foreach (var leftNode in leftSubtree)
                        foreach (var rightNode in rightSubtree)
                        {
                            var pathNode = GetPathNode(leftNode.Id, rightNode.Id);
                            depth = Math.Min(depth, pathNode.Depth);

                            if (depth == 0)
                                return;
                        }
                }
            }

            optimizedEnd[root.Id] = depth;
        }

        protected override TreeNode GetPathNode(int from, int to)
        {
            var lowestCommonAncestor = GetLowestCommonAncestor(from, to);

            var current = TreeNodes[lowestCommonAncestor];


            var pathNode = current;
            var pathLength = int.MaxValue;

            optimizedEnd.TryGetValue(lowestCommonAncestor, out var optimizedDepth);

            while (current != null && current.Depth >= optimizedDepth)
            {
                var subIndex = Index[current.Id];
                var path1Length = subIndex[from].Length;
                var path2Length = subIndex[to].Length;

                var newPathLength = path1Length + path2Length;
                if (newPathLength < pathLength && path1Length != int.MaxValue && path2Length != int.MaxValue)
                {
                    pathLength = newPathLength;
                    pathNode = current;
                }
                current = current.Parent;
            }

            return pathNode;
        }

        public override int Size { get { return base.Size + optimizedEnd.Count; } }
    }
}
