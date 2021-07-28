using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using System.Linq;
using Thesis.Trees;

namespace Thesis.NegativeCycleDetection
{
    public class NegativeCycleDetectionTreedepth : DivideAndConquer<NegativeCycleDetectionResult>
    {
        private Dictionary<int, int> potential = new Dictionary<int, int>();

        protected override void Initialize(IEnumerable<INode> nodes, IEnumerable<IEdge> edges, TreeNode treeNode, IEnumerable<TreeNode> treenodes)
        {
            base.Initialize(nodes, edges, treeNode, treenodes);
            foreach (var node in nodes)
                potential.Add(node.Id, 0);
        }
        protected override NegativeCycleDetectionResult Increment(List<Node> nodes, Node addedNode, TreeNode root, NegativeCycleDetectionResult previousResult)
        {
            if (previousResult.NegativeCycleDetected)
                return previousResult;
            if (nodes.Count <= 1)
                return new NegativeCycleDetectionResult();

            var x = addedNode;
            var G_ = Compute_G_(out var neighbours, out var incomingEdges, nodes, x);

            var W = Compute_W(out var p_, x);
            var d = Compute_d(x, G_);
            var R = Compute_R(G_, d);

            if (DetectNegativeCycle(out var G, neighbours, incomingEdges, x, G_, d, R))
                return new NegativeCycleDetectionResult() { NegativeCycleDetected = true };

            var D = Compute_D(R, d);
            var p = Compute_p(G_, D, d);

            return new NegativeCycleDetectionResult();
        }

        private List<Node> Compute_G_(out List<Node> neighbours, out List<Edge> incomingEdges, List<Node> nodes, Node x)
        {
            // Remove all incoming edges from x

            incomingEdges = x.GetIncomingEdges();
            neighbours = new List<Node>();
            for (var i = x.Edges.Count - 1; i >= 0; i--)
            {
                var edge = x.Edges[i];
                var neighbour = edge.Other(x);
                neighbours.Add(neighbour);
                neighbour.Edges.Remove(edge);
                x.Edges.Remove(edge);
            }
            return nodes;
        }

        private int Compute_W(out Dictionary<int,int> p_, Node x)
        {
            p_ = potential;
            var W = 0;
            var outgoingEdges = x.GetOutgoingEdges();

            foreach (var edge in outgoingEdges)
                W = Math.Max(W, potential[edge.Node2.Id] - Edges[edge.Id].Weight);
            potential[x.Id] = W;
            return W;
        }

        private Dictionary<int, int> Compute_d(Node x, List<Node> G_)
        {
            Dijkstra.Dijkstra.DijkstraAlgorithm(out var d, out var prev, G_, x, true, (v, w, e)=> { return W_p(Edges[e.Id]); });
            return d;
        }

        private List<Node> Compute_R(List<Node> G_, Dictionary<int, int> d)
        {
            var R = new List<Node>();
            foreach (var node in G_)
                if (d[node.Id] != int.MaxValue)
                    R.Add(node);

            return R;
        }

        private bool DetectNegativeCycle(out List<Node> G, List<Node> neighbours, List<Edge> incomingEdges, Node x, List<Node> G_, Dictionary<int, int> d, List<Node> R) 
        {
            G = G_;

            // Add incoming edges back
            foreach (var neighbour in neighbours)
                neighbour.Edges.AddRange(incomingEdges.Where(e => e.Node1 == neighbour));
            x.Edges.AddRange(incomingEdges);

            foreach (var vx in incomingEdges)
            {
                var v = vx.Node1.Id;
                if (d[v] != int.MaxValue && d[v] + W_p(vx) < 0)
                    return true;
            }

            return false;
        }

        private int Compute_D(List<Node> R, Dictionary<int,int> d)
        {
            var D = 0;

            foreach (var v in R)
                foreach (var uv in v.Edges)
                    if (uv.Node2 == v && d[uv.Node1.Id] == int.MaxValue)                    
                        D = Math.Max(D, d[v.Id] - W_p(uv));
            return D;
        }

        private Dictionary<int,int> Compute_p(List<Node> G, int D, Dictionary<int,int> d)
        {
            foreach (var v in G)
            {
                if (d[v.Id] == int.MaxValue)
                    potential[v.Id] += D;
                else
                    potential[v.Id] += d[v.Id];
            }
            return potential;
        }

        private int W_p(Edge xy) => xy.Weight + potential[xy.Node1.Id] - potential[xy.Node2.Id];

        protected override NegativeCycleDetectionResult Union(List<Tuple<TreeNode, NegativeCycleDetectionResult>> results)
        {
            foreach (var result in results)
            {
                if (result.Item2.NegativeCycleDetected)
                    return result.Item2;
            }

            return new NegativeCycleDetectionResult();
        }
    }
}
