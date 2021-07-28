using System;
using System.Collections.Generic;
using RangeMinimumQuery;

namespace LowestCommonAncestor
{
    public class LCA<T> where T : ITreeNode<T>
    {
        private int[] Nodes;
        private int[] Depth;
        private Dictionary<int, int> First;
        private Dictionary<int, T> N = new Dictionary<int, T>();
        private RMQ RMQ { get; set; }

        public LCA(T root)
        {
            GetNodes(root);
            var n = N.Count;
            Nodes = new int[2 * n - 1];
            Depth = new int[2 * n - 1];
            First = new Dictionary<int, int>();
            var position = 0;
            DFS(root, ref position);
            RMQ = new RMQ(Depth);
        }

        private void GetNodes(T root)
        {
            var nodes = new List<T>();
            nodes.Add(root);
            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                N.Add(node.Id, node);
                foreach (var child in node.Children)
                    nodes.Add(child);
            }            
        }

        private void DFS(T node, ref int position, int depth = 0)
        {
            First[node.Id] = position;
            Update(node, ref position, depth);
            foreach (var child in node.Children)
            {
                DFS(child, ref position, depth + 1);
                Update(node, ref position, depth);
            }
            
        }

        private void Update(T node, ref int position, int depth)
        {
            Nodes[position] = node.Id;
            Depth[position] = depth;
            position++;
        }

        public T Query(T node1, T node2)
        {
            var pos1 = First[node1.Id];
            var pos2 = First[node2.Id];
            var left = Math.Min(pos1, pos2);
            var right = Math.Max(pos1, pos2);
            RMQ.Query(left, right, out var index, out _);
            return N[Nodes[index]];
        }
    }
}
