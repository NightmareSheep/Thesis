using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using Thesis.Trees;

namespace Thesis.TwoHopCover.Treedepth
{
    public class TwoHopCoverTreedepth : DivideAndConquer<TwoHopCoverResult>
    {
        public Dictionary<int, Dictionary<int, Entry>> Index { get; set; }
        private TreeNode root;

        protected override void Initialize(IEnumerable<INode> nodes, IEnumerable<IEdge> edges, TreeNode treeNode, IEnumerable<TreeNode> treenodes)
        {
            base.Initialize(nodes, edges, treeNode, treenodes);
            Index = new Dictionary<int, Dictionary<int, Entry>>();
            this.root = treeNode;
        }

        protected override TwoHopCoverResult Increment(List<Node> nodes, Node addedNode, TreeNode root, TwoHopCoverResult previousResult)
        {
            var subindex = new Dictionary<int, Entry>();
            Index[root.Id] = subindex;
            subindex[root.Id] = new Entry() { To = root.Id, Length = 0 };
            Dijkstra.Dijkstra.DijkstraAlgorithm(out var dist, out var prev, nodes, addedNode);
            foreach (var node in nodes)
                subindex[node.Id] = new Entry() { To = node.Id, Length = dist[node.Id], Middle = prev[node.Id] };

            return new TwoHopCoverResult(TreeNodes, Index, Nodes, this.root);
        }

        protected override TwoHopCoverResult Union(List<Tuple<TreeNode, TwoHopCoverResult>> results)
        {
            return new TwoHopCoverResult(TreeNodes, Index, Nodes, root);
        }
    }
}
