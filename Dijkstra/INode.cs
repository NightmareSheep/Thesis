using System;
using System.Collections.Generic;
using System.Text;

namespace Dijkstra
{
    public interface INode
    {
        int Id { get; }
        IEnumerable<IEdge> Edges { get; }
    }
}
