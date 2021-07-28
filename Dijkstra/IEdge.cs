using System;
using System.Collections.Generic;
using System.Text;

namespace Dijkstra
{
    public interface IEdge
    {
        int Id { get; }
        int Weight { get; }
        INode Node1 { get; }
        INode Node2 { get; }
    }
}
