using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Thesis.Graph;

namespace Thesis
{
    public class GraphLoader
    {
        public void LoadGraph(TextReader tr, out Node[] nodes, out Edge[] edges)
        {
            string line;
            while ((line = tr.ReadLine())[0] != 'p'){}
            InitializeGraph(line, out nodes, out edges);

            var edgeId = 0;
            while ((line = tr.ReadLine()) != null)
                if (line[0] != 'c')
                {
                    AddEdge(line, nodes, edges, edgeId);
                    edgeId++;
                }
        }

        public void LoadGraph(int numberOfNodes, Tuple<int, int>[] edgeTuples, out Node[] nodes, out Edge[] edges)
        {
            nodes = new Node[numberOfNodes];
            for (int i = 0; i < nodes.Length; i++)
                nodes[i] = new Node() { Id = i };

            edges = new Edge[edgeTuples.Length];

            for (int i = 0; i < edgeTuples.Length; i++)
            {
                var edge = new Edge() { Id = i };
                var tuple = edgeTuples[i];
                edges[i] = edge;
                var node1 = nodes[tuple.Item1];
                var node2 = nodes[tuple.Item2];
                edge.Node1 = node1;
                edge.Node2 = node2;
                node1.Edges.Add(edge);
                node2.Edges.Add(edge);
            }
        }

        private void InitializeGraph(string line, out Node[] nodes, out Edge[] edges)
        {
            var words = line.Split(' ');
            var nrOfNodes = int.Parse(words[3]);
            var nrOfEdges = int.Parse(words[4]);

            nodes = new Node[nrOfNodes];
            edges = new Edge[nrOfEdges];
            for (int i = 0; i < nrOfNodes; i++)
                nodes[i] = new Node() { Id = i };
            for (int i = 0; i < nrOfEdges; i++)
                edges[i] = new Edge() { Id = i };
        }

        private void AddEdge(string line, Node[] nodes, Edge[] edges, int edgeId)
        {
            var words = line.Split(' ');
            var n1 = int.Parse(words[0]);
            var n2 = int.Parse(words[1]);
            var edge = edges[edgeId];
            var node1 = nodes[n1 - 1];
            var node2 = nodes[n2 - 1];
            edge.Node1 = node1;
            edge.Node2 = node2;
            node1.Edges.Add(edge);
            node2.Edges.Add(edge);
        }


    }
}
