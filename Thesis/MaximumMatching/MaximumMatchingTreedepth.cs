using System;
using System.Collections.Generic;
using Thesis.Graph;
using System.Linq;
using Thesis.Trees;

namespace Thesis.MaximumMatching
{
    public class MaximumMatchingTreedepth : DivideAndConquer<MaximumMatchingResult>
    {
        
        private Entry[] Entries;

        private Node[] NodesClone { get; set; }
        private Edge[] EdgesClone { get; set; }

        protected override void Initialize(IEnumerable<INode> nodes, IEnumerable<IEdge> edges, TreeNode treeNode, IEnumerable<TreeNode> treeNodes)
        {
            base.Initialize(nodes, edges, treeNode, treeNodes);
            Entries = new Entry[nodes.Count() + 1];
            foreach (var node in nodes)
            {
                Entries[node.Id] = new Entry();
            }
            Entries[^1] = new Entry();

            NodesClone = new Node[nodes.Count()];
            EdgesClone = new Edge[edges.Count()];
        }

        protected override MaximumMatchingResult Increment(List<Node> nodes, Node n, TreeNode addedNode, MaximumMatchingResult previousResult)
        {
            ExpandClone(addedNode);
            var unMatchedVertexIds = new List<int> { addedNode.Id };
            unMatchedVertexIds.AddRange(previousResult.UnMatchedVertexIds);

            foreach (var vertexId in unMatchedVertexIds)
                SearchForAugmentPath(vertexId);

            return new MaximumMatchingResult(){ UnMatchedVertexIds = unMatchedVertexIds, Entries = Entries };
        }

        private void ExpandClone(TreeNode addedNode)
        {
            var Node = new Node { Id = addedNode.Id };
            NodesClone[Node.Id] = Node;
            foreach (var edge in Nodes[addedNode.Id].Edges)
            {
                var neighbour = edge.Node1.Id != addedNode.Id ? edge.Node1 : edge.Node2;
                if (TreeNodes[neighbour.Id].Depth > addedNode.Depth)
                {
                    var NeighbourNode = new Node { Id = neighbour.Id };
                    var EdgeToNeighbour = new Edge { Id = edge.Id, Node1 = Node, Node2 = NeighbourNode };
                    Node.Edges.Add(EdgeToNeighbour);
                    NeighbourNode.Edges.Add(EdgeToNeighbour);
                    EdgesClone[EdgeToNeighbour.Id] = EdgeToNeighbour;
                    NodesClone[NeighbourNode.Id] = NeighbourNode;
                }
            }
        }

        private void SearchForAugmentPath(int vertexId)
        {
            var EdgeQueue = new Queue<Tuple<INode, IEdge>>();
            var startNode = NodesClone[vertexId];

            foreach (var edge in startNode.Edges)
                EdgeQueue.Enqueue(new Tuple<INode, IEdge>(startNode, edge));

            while (EdgeQueue.Count > 0)
            {
                var tupple = EdgeQueue.Dequeue();
                var x = tupple.Item1;
                var xy = tupple.Item2;
                var y = xy.Node1.Id == x.Id ? xy.Node2 : xy.Node1;

                if (Entries[y.Id].Mate == -1)
                {
                    Augment(x.Id, y.Id, xy.Id);
                    break;
                }

                if (Entries[y.Id].LabelType != LabelType.NonOuter)
                    AssignEdgeLabels(EdgeQueue, x, y, xy);
                else
                    AssignVertexLabel(EdgeQueue, x, y, xy);
            }

            foreach (var entry in Entries)
                entry.LabelType = LabelType.NonOuter;
        }

        private void Augment(int v, int w, int vw)
        {
            var t = Entries[v].Mate;
            Entries[v].Mate = w;
            Entries[v].Edge = vw;
            if (t == -1 || Entries[t].Mate != v)
                return;

            if (Entries[v].LabelType == LabelType.Vertex)
            {
                Entries[t].Mate = Entries[v].Vertex;
                Augment(Entries[v].Vertex, t, Entries[v].vertexLabelEdge);
            }
            else
            {
                var edge = EdgesClone[Entries[v].Edge];
                var x = edge.Node1.Id;
                var y = edge.Node2.Id;
                Augment(x, y, edge.Id);
                Augment(y, x, edge.Id);
            }
        }

        private void AssignEdgeLabels(Queue<Tuple<INode, IEdge>> edgeQueue, INode x, INode y, IEdge xy)
        {
            var r = Entries[x.Id].First;
            var s = Entries[y.Id].First;
            if (r == s)
                return;

            List<int> flagged = new List<int>();
            flagged.Add(r);
            flagged.Add(s);

            while (r != s)
            {
                var temp = r;
                r = s;
                s = temp;

                r = Entries[Entries[Entries[r].Mate].Vertex].First;
                flagged.Add(r);
            }
            var join = r;

            foreach (var vertexId in flagged)
            {
                if (vertexId != join)
                {
                    var entry = Entries[vertexId];
                    entry.LabelType = LabelType.Edge;
                    entry.Edge = xy.Id;
                    entry.First = join;
                    var node = NodesClone[vertexId];
                    foreach (var edge in node.Edges)
                        edgeQueue.Enqueue(new Tuple<INode, IEdge>(node, edge));
                }
            }

            foreach (var entry in Entries)
                if (Entries[entry.First].LabelType != LabelType.NonOuter)
                    entry.First = join;
        }

        private void AssignVertexLabel(Queue<Tuple<INode, IEdge>> edgeQueue, INode x, INode y, IEdge xy)
        {
            var v = Entries[y.Id].Mate;
            var vNode = NodesClone[v];
            var vEntry = Entries[v];
            if (vEntry.LabelType == LabelType.NonOuter)
            {
                vEntry.LabelType = LabelType.Vertex;
                vEntry.vertexLabelEdge = xy.Id;
                vEntry.Vertex = x.Id;
                vEntry.First = y.Id;
                foreach (var edge in vNode.Edges)
                    edgeQueue.Enqueue(new Tuple<INode, IEdge>(vNode, edge));
            }
        }

        protected override MaximumMatchingResult Union(List<Tuple<TreeNode, MaximumMatchingResult>> results)
        {
            var union = new MaximumMatchingResult();
            union.Entries = Entries;

            foreach (var result in results)
                union.UnMatchedVertexIds.AddRange(result.Item2.UnMatchedVertexIds);

            return union;
        }
    }
}
