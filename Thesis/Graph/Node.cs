using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Thesis.Graph
{
    public class Node : INode, Dijkstra.INode
    {
        public int Id { get; set; }
        public List<Edge> Edges { get; set; } = new List<Edge>();

        IEnumerable<Dijkstra.IEdge> Dijkstra.INode.Edges => Edges;
        IEnumerable<IEdge> INode.Edges => Edges;
    }
}
