using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dijkstra
{
    public static class Extensions
    {
        public static INode Other(this IEdge edge, INode node) => edge.Node1.Equals(node) ? edge.Node2 : edge.Node1;
        public static List<IEdge> OutgoingEdges(this INode node) => node.Edges.Where(e => e.Node1 == node).ToList();
    }
}
