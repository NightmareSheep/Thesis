using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;
using System.Linq;
using Thesis.Trees;

namespace Thesis
{
    public static class Extensions
    {
        public static List<TreeNode> GetTreeNodesInSubtree(this TreeNode node, List<TreeNode> list = null)
        {
            list ??= new List<TreeNode>();
            list.Add(node);
            foreach (var child in node.Children)
                GetTreeNodesInSubtree(child, list);

            return list;
        }


        public static Node Other(this Edge edge, Node node)
        {
            return edge.Node1.Equals(node) ? edge.Node2 : edge.Node1;
        }

        public static List<Node> GetNodesLegacy(this TreeNode treeNode, Dictionary<int, Node> nodes, List<Node> list = null)
        {
            list ??= new List<Node>();

            list.Add(nodes[treeNode.Id]);
            foreach(var child in treeNode.Children)
                child.GetNodesLegacy(nodes, list);

            return list;
        }

        public static List<Node> GetNodes(this TreeNode root, Dictionary<int, Node> nodes)
        {
            var resultNodes = new List<Node>();
            var treeNodes = new List<TreeNode>();
            treeNodes.Add(root);
            for (var i = 0; i < treeNodes.Count; i++)
            {
                var treeNode = treeNodes[i];
                resultNodes.Add(nodes[treeNode.Id]);
                foreach (var child in treeNode.Children)
                    treeNodes.Add(child);
            }

            return resultNodes;
        }

        public static List<Edge> GetOutgoingEdges(this Node node) => node.Edges.Where(e => e.Node1 == node).ToList();
        public static List<Edge> GetIncomingEdges(this Node node) => node.Edges.Where(e => e.Node2 == node).ToList();

        public static List<int> GetEliminationOrder(this TreeNode root, List<int> list = null)
        {
            var result = new List<int>();
            var treeNodes = new List<TreeNode>();
            treeNodes.Add(root);

            for (var i = 0; i < treeNodes.Count; i++)
            {
                var treeNode = treeNodes[i];
                result.Add(treeNode.Id);
                foreach (var child in treeNode.Children)
                    treeNodes.Add(child);
            }
            return result;
        }
    }
}
