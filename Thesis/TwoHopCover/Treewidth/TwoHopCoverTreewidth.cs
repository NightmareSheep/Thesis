using C5;
using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using System.Linq;

namespace Thesis.TwoHopCover.Treewidth
{
    public class TwoHopCoverTreewidth
    {
        private Dictionary<int, Node> Vertices { get; set; } = new Dictionary<int, Node>();
        private Dictionary<int, Edge> Edges { get; set; } = new Dictionary<int, Edge>();

        private Index Index { get; set; } = new Index();

        public Index ComputeIndex(IEnumerable<INode> nodes, IEnumerable<IEdge> edges, List<int> eliminationOrder)
        {
            LoadGraph(nodes, edges);
            var numberOfNodes = nodes.Count();

            foreach (var node in nodes)
                Index.AddOrUpdate(node.Id, node.Id, 0, -1);

            for (var i = 0; i < eliminationOrder.Count; i++)
            {
                var v = Vertices[eliminationOrder[i]];

                var handles = new Dictionary<int, IPriorityQueueHandle<Dijkstra.NodeDistance>>();
                var priorityQueue = new IntervalHeap<Dijkstra.NodeDistance>(numberOfNodes);
                var start = new Dijkstra.NodeDistance(v, 0);
                IPriorityQueueHandle<Dijkstra.NodeDistance> handle = null;
                priorityQueue.Add(ref handle, start);
                handles.Add(v.Id, handle);

                while (priorityQueue.Count > 0)
                {
                    var u = priorityQueue.DeleteMin();
                    handles.Remove(u.Node.Id);
                    foreach (var edge in u.Node.Edges)
                    {
                        var neighbour = edge.Node1 == u.Node ? edge.Node2 : edge.Node1;
                        var distance = u.Distance + edge.Weight;
                        Index.Query(out var indexDistance, v.Id, neighbour.Id);
                        if (distance < indexDistance)
                        {
                            Index.AddOrUpdate(neighbour.Id, v.Id, distance, u.Node.Id != v.Id ? u.Node.Id : -1);

                            handles.TryGetValue(neighbour.Id, out var neighbourHandle);
                            if (neighbourHandle != null)
                                priorityQueue.Delete(neighbourHandle);
                            IPriorityQueueHandle<Dijkstra.NodeDistance> newHandle = null;
                            priorityQueue.Add(ref newHandle, new Dijkstra.NodeDistance(neighbour, distance));
                            handles[neighbour.Id] = newHandle;
                        }
                    }
                }
            }

            return Index;
        }

        private void LoadGraph(IEnumerable<INode> V, IEnumerable<IEdge> E)
        {
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
        }
    }
}
