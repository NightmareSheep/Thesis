using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using System.Linq;

namespace Thesis
{
    public class EdgeHelper
    {
        Dictionary<int, Dictionary<int, Edge>> EdgesOutgoing;
        Dictionary<int, Dictionary<int, Edge>> EdgesIncoming;

        public EdgeHelper(List<Edge> edges)
        {
            EdgesOutgoing = new Dictionary<int, Dictionary<int, Edge>>(edges.Count);
            EdgesIncoming = new Dictionary<int, Dictionary<int, Edge>>(edges.Count);

            foreach (var edge in edges)
            {
                if (!EdgesOutgoing.ContainsKey(edge.Node1.Id))
                    EdgesOutgoing.Add(edge.Node1.Id, new Dictionary<int, Edge>());

                var subIndex = EdgesOutgoing[edge.Node1.Id];
                subIndex[edge.Node2.Id] = edge;

                if (!EdgesIncoming.ContainsKey(edge.Node2.Id))
                    EdgesIncoming.Add(edge.Node2.Id, new Dictionary<int, Edge>());

                subIndex = EdgesIncoming[edge.Node2.Id];
                subIndex[edge.Node1.Id] = edge;
            }
        }

        public bool TryGetEdge(int node1, int node2, out Edge edge)
        {
            if (EdgesOutgoing.TryGetValue(node1, out var subindex) && subindex.TryGetValue(node2, out edge))
            {
                return true;
            }

            edge = null;
            return false;
        }

        public List<Edge> GetOutgoingEdges(int v)
        {
            EdgesOutgoing.TryGetValue(v, out var subIndex);

            if (subIndex == null)
                return new List<Edge>();

            return subIndex.Values.ToList();
        }

        public List<Edge> GetIncomingEdges(int v)
        {
            EdgesIncoming.TryGetValue(v, out var subIndex);

            if (subIndex == null)
                return new List<Edge>();

            return subIndex.Values.ToList();
        }
    }
}
