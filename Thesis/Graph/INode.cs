using System;
using System.Collections.Generic;
using System.Text;

namespace Thesis.Graph
{
    public interface INode
    {
        int Id { get; }
        IEnumerable<IEdge> Edges { get; }
    }
}
