using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using System.Linq;
using Thesis.Trees;

namespace Thesis.NegativeCycleDetection
{
    public class DirectedPathConsistency
    {
        private Dictionary<int, Node> Vertices { get; set; }
        private Dictionary<int, Edge> Edges { get; set; }
        private int Counter { get; set; }

        public bool CheckConsistency(IEnumerable<INode> V, IEnumerable<IEdge> E, TreeNode root)
        {
            List<int> d = new List<int>();
            AddNodeToList(root, d);
            d.Reverse();

            return CheckConsistency(V, E, d);
        }

        private void AddNodeToList(TreeNode node, List<int> d)
        {
            d.Add(node.Id);
            foreach(var child in node.Children)
                AddNodeToList(child, d);
        }

        public bool CheckConsistency(IEnumerable<INode> V, IEnumerable<IEdge> E, List<int> d)
        {
            if (V.Count() != d.Count)
                throw new Exception("Number of vertices and ordering must have the same number of elements");

            LoadGraph(V, E);
            var edgeHelper = new EdgeHelper(Edges.Values.ToList());

            for (var k = V.Count() - 1; k >= 0; k--)
            {
                var v_k = Vertices[d[k]];

                var outgoingEdges = edgeHelper.GetOutgoingEdges(v_k.Id);

                var doubleEdges = new List<Tuple<Edge, Edge>>();

                foreach (var outgoingEdge in outgoingEdges)
                    if (edgeHelper.TryGetEdge(outgoingEdge.Node2.Id, outgoingEdge.Node1.Id, out var incomingEdge))
                        doubleEdges.Add(new Tuple<Edge, Edge>(incomingEdge, outgoingEdge));

                for (var i = 0; i < doubleEdges.Count; i++)
                    for (var j = i + 1; j < doubleEdges.Count; j++)
                    {
                        // Tupple Item1 is an incoming edge to k, Tupple Item2 is an outgoing edge from k

                        var e_ik = doubleEdges[i];
                        var e_jk = doubleEdges[j];

                        var v_i = e_ik.Item1.Node1;
                        var v_j = e_ik.Item2.Node2;
                        edgeHelper.TryGetEdge(v_i.Id, v_j.Id, out var e_ij);
                        edgeHelper.TryGetEdge(v_j.Id, v_i.Id, out var e_ji);

                        if (e_ij == null)
                        {
                            Counter++;
                            e_ij = new Edge() { Id = Counter, Node1 = v_i, Node2 = v_j, Weight = int.MaxValue };
                            Edges.Add(Counter, e_ij);
                            v_i.Edges.Add(e_ij);
                            v_j.Edges.Add(e_ij);
                        }

                        if (e_ji == null)
                        {
                            Counter++;
                            e_ji = new Edge() { Id = Counter, Node1 = v_j, Node2 = v_i, Weight = int.MaxValue };
                            Edges.Add(Counter, e_ji);
                            v_i.Edges.Add(e_ji);
                            v_j.Edges.Add(e_ji);
                        }

                        e_ij.Weight = Math.Min(e_ij.Weight, e_ik.Item1.Weight + e_jk.Item2.Weight);
                        e_ji.Weight = Math.Min(e_ji.Weight, e_jk.Item1.Weight + e_ik.Item2.Weight);

                        if (e_ij.Weight + e_ji.Weight < 0)
                            return false;

                    }
            }

            return true;
        }

        private void LoadGraph(IEnumerable<INode> V, IEnumerable<IEdge> E)
        {
            Vertices = new Dictionary<int, Node>(V.Count());
            Edges = new Dictionary<int, Edge>(E.Count());

            // Make a copy of the input graph
            foreach (var node in V)
                Vertices[node.Id] = new Node() { Id = node.Id };

            foreach (var edge in E)
            {
                var node1 = Vertices[edge.Node1.Id];
                var node2 = Vertices[edge.Node2.Id];
                var newEdge = new Edge() { Id = edge.Id, Node1 = node1, Node2 = node2, Weight = edge.Weight };
                Edges[edge.Id] = newEdge;
                node1.Edges.Add(newEdge);
                node2.Edges.Add(newEdge);
            }

            Counter = E.Count() + 1;
        }
    }
}
