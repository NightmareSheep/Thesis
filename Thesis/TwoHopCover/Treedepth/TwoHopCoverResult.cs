using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using LowestCommonAncestor;
using System.Linq;
using Thesis.Trees;

namespace Thesis.TwoHopCover.Treedepth
{
    public class TwoHopCoverResult : IIndex
    {
        protected Dictionary<int, Node> nodes;
        protected TreeNode root;
        protected Dictionary<int, TreeNode> TreeNodes { get; set; }
        protected Dictionary<int, Dictionary<int, Entry>> Index { get; set; }
        protected LCA<TreeNode> LCA { get; set; }

        public TwoHopCoverResult(Dictionary<int, TreeNode> treeNodes, Dictionary<int, Dictionary<int, Entry>> index, Dictionary<int, Node> nodes, TreeNode root)
        {
            TreeNodes = treeNodes;
            Index = index;
            this.nodes = nodes;
            this.root = root;
        }

        public virtual int[] Query(int from, int to)
        {
            var pathNode = GetPathNode(from, to);
            var path1 = GetPath(pathNode.Id, from);
            path1.Reverse();
            var path2 = GetPath(pathNode.Id, to);
            

            var path = new List<int>();
            path.AddRange(path1);
            path.Add(pathNode.Id);
            path.AddRange(path2);

            return path.ToArray();
        }

        protected virtual TreeNode GetPathNode(int from, int to)
        {
            var lowestCommonAncestor = GetLowestCommonAncestor(from, to);

            var current = TreeNodes[lowestCommonAncestor];
            var pathNode = current;
            var pathLength = int.MaxValue;
            
            while (current != null)
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

        protected List<int> GetPath(int from, int to)
        {
            var path = new List<int>();

            var subIndex = Index[from];

            var current = to;
            while (current != from)
            {
                path.Add(current);
                current = subIndex[current].Middle;
            }
            path.Reverse();
            return path;
        }

        protected int GetLowestCommonAncestor(int node1, int node2)
        {
            LCA ??= new LCA<TreeNode>(TreeNodes.Values.FirstOrDefault(v => v.Depth == 0));
            var result = LCA.Query(TreeNodes[node1], TreeNodes[node2]);
            return result.Id;
        }

        public EnhancedTwoHopCoverResult GetEnhancedResult()
        {
            return new EnhancedTwoHopCoverResult(TreeNodes, Index, nodes, root);
        }

        public EnhancedTwoHopCoverResult2 GetEnhancedResult2()
        {
            return new EnhancedTwoHopCoverResult2(TreeNodes, Index, nodes, root);
        }

        public EnhancedTwoHopCoverResult3 GetEnhancedResult3()
        {
            return new EnhancedTwoHopCoverResult3(TreeNodes, Index, nodes, root);
        }

        public virtual int Size { 
            get {
                var size = 0;
                foreach (var value in this.Index.Values)
                    size += value.Keys.Count;
                return size; 
            } 
        }
    }
}
