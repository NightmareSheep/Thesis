using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using System.Linq;
using Thesis.Trees;

namespace Thesis
{
    public abstract class DivideAndConquer<ResultType>
    {
        protected Dictionary<int, Node> Nodes { get; set; } = new Dictionary<int, Node>();
        protected Dictionary<int, Edge> Edges { get; set; } = new Dictionary<int, Edge>();
        protected Dictionary<int, TreeNode> TreeNodes { get; set; } = new Dictionary<int, TreeNode>();

        public DivideAndConquer()
        {
        }

        protected virtual void Initialize(IEnumerable<INode> nodes, IEnumerable<IEdge> edges, TreeNode treeNode, IEnumerable<TreeNode> treenodes) 
        {
            // Make a copy of the input graph
            foreach (var node in nodes)
                Nodes[node.Id] = new Node() { Id = node.Id };

            foreach (var edge in edges)
            {
                var node1 = Nodes[edge.Node1.Id];
                var node2 = Nodes[edge.Node2.Id];
                var newEdge = new Edge() { Id = edge.Id, Node1 = node1, Node2 = node2, Weight = edge.Weight };
                Edges[edge.Id] = newEdge;
                node1.Edges.Add(newEdge);
                node2.Edges.Add(newEdge);
            }

            foreach (var treenode in treenodes)
                TreeNodes[treenode.Id] = treenode;
        } 

        public ResultType Compute(IEnumerable<INode> nodes, IEnumerable<IEdge> edges, TreeNode root, IEnumerable<TreeNode> treenodes)
        {
            Initialize(nodes, edges, root, treenodes);
            return Compute(Nodes.Values.ToList(), root);
        }

        private ResultType ComputeLegacy(List<Node> nodes, TreeNode treeNode)
        {
            var results = new List<Tuple<TreeNode, ResultType>>();

            RemoveNode(Nodes[treeNode.Id]);
            foreach (var child in treeNode.Children)
                results.Add(new Tuple<TreeNode, ResultType>(child, Compute(child.GetNodes(Nodes) ,child)));
            AddNode(Nodes[treeNode.Id]);

            var childrenUnionResult = Union(results);            
            return Increment(nodes, Nodes[treeNode.Id], treeNode, childrenUnionResult);
        }

        private ResultType Compute(List<Node> nodes, TreeNode root)
        {
            var results = new Dictionary<int,ResultType>();
            var elimOrder = root.GetEliminationOrder();
            for (var i = 0; i < elimOrder.Count; i++)
            {
                var nodeId = elimOrder[i];
                RemoveNode(Nodes[nodeId]);
            }
            elimOrder.Reverse();
            for (var i = 0; i < elimOrder.Count; i++)
            {
                var nodeId = elimOrder[i];
                AddNode(Nodes[nodeId]);
                var node = Nodes[nodeId];
                var treeNode = TreeNodes[nodeId];
                var childrenResults = new List<Tuple<TreeNode, ResultType>>();
                foreach (var child in treeNode.Children)
                    childrenResults.Add(new Tuple<TreeNode, ResultType>(child, results[child.Id]));
                var childrenUnionResult = Union(childrenResults);
                var result = Increment(treeNode.GetNodes(Nodes), node, treeNode, childrenUnionResult);
                results.Add(treeNode.Id, result);
            }

            return results[root.Id];
        }

        private void RemoveNode(Node node)
        {
            foreach (var edge in node.Edges)
            {
                var other = edge.Other(node);
                other.Edges.Remove(edge);
            }
        }

        private void AddNode(Node node)
        {
            foreach (var edge in node.Edges)
            {
                var other = edge.Other(node);
                other.Edges.Add(edge);
            }
        }

        protected abstract ResultType Increment(List<Node> nodes, Node addedNode, TreeNode root, ResultType previousResult);

        protected abstract ResultType Union(List<Tuple<TreeNode, ResultType>> results);
    }
}
