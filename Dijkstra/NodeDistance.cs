using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace Dijkstra
{
    public struct NodeDistance : IComparable<NodeDistance>
    {
        public NodeDistance(INode node, int distance)
        {
            Node = node;
            Distance = distance;
        }

        public INode Node;
        public int Distance;

        public int CompareTo(NodeDistance obj) => Distance.CompareTo(obj.Distance);
        public bool Equals(NodeDistance obj) => Distance == obj.Distance;

    }
}
