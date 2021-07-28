using System;
using System.Collections.Generic;
using System.Text;

namespace Thesis.Graph
{
    public class Edge : IEdge, Dijkstra.IEdge
    {
        public int Id { get; set; }

        public int Weight { get; set; } = 1;
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }


        INode IEdge.Node1 => Node1;
        INode IEdge.Node2 => Node2;
        Dijkstra.INode Dijkstra.IEdge.Node1 => Node1;
        Dijkstra.INode Dijkstra.IEdge.Node2 => Node2;
    }
}
