using System;
using System.Collections.Generic;
using System.Linq;
using C5;

namespace Dijkstra
{
    public static class Dijkstra
    {
        public static void DijkstraAlgorithm(out Dictionary<int, int> dist, out Dictionary<int, int> prev, IEnumerable<INode> nodes, INode start, bool directed = false, Func<INode, INode, IEdge, int> DistanceFunction = null)
        {
            dist = new Dictionary<int, int>();
            prev = new Dictionary<int, int>();
            var handles = new Dictionary<int,IPriorityQueueHandle<NodeDistance>>();
            var priorityQueue = new IntervalHeap<NodeDistance>(nodes.Count());

            foreach (var node in nodes)
            {                
                dist[node.Id] = int.MaxValue;
                prev[node.Id] = -1;
                IPriorityQueueHandle<NodeDistance> handle = null;
                priorityQueue.Add(ref handle, new NodeDistance(node, int.MaxValue));
                handles[node.Id] = handle;
            }
            dist[start.Id] = 0;
            priorityQueue.Replace(handles[start.Id], new NodeDistance(start, 0));

            while (priorityQueue.Count > 0)
            {
                var u = priorityQueue.DeleteMin();
                if (u.Distance == int.MaxValue)
                    break;

                handles.Remove(u.Node.Id);
                var edges = directed ? u.Node.OutgoingEdges() : u.Node.Edges;

                foreach (var edge in edges)
                {
                    var neighbour = edge.Other(u.Node);
                    handles.TryGetValue(neighbour.Id, out var handle);
                    var distanceToNeighbour = DistanceFunction == null ? u.Distance + edge.Weight : u.Distance + DistanceFunction(u.Node, neighbour, edge);
                    if (distanceToNeighbour < dist[neighbour.Id] && handle != null)
                    {
                        priorityQueue.Replace(handle, new NodeDistance(neighbour, distanceToNeighbour));
                        dist[neighbour.Id] = distanceToNeighbour;
                        prev[neighbour.Id] = u.Node.Id;
                    }
                }
            }
        }
    }
}
