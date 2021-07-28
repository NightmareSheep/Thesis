using System;
using Thesis.Graph;
using Thesis.MaximumMatching;
using Thesis.NegativeCycleDetection;
using Thesis.TwoHopCover.Treedepth;
using Thesis.TwoHopCover.Treewidth;
using System.Linq;
using System.Collections.Generic;
using LowestCommonAncestor;
using Thesis.Trees;
using System.IO;
using System.Diagnostics;

namespace Thesis
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestMaximumMatching();
            //TestNegativeCycleDetection();
            TestTwoHopCover();
            //TestLowestCommonAncestor();

            //DoWork();

            Console.ReadLine();
            Console.WriteLine("Hello World!");
        }

        private static void TestMaximumMatching()
        {
            var nodes = new Node[10];
            for (int i = 0; i < nodes.Length; i++)
                nodes[i] = new Node { Id = i };
            var edges = new Edge[12];
            for (int i = 0; i < edges.Length; i++)
                edges[i] = new Edge { Id = i };

            nodes[0].Edges.Add(edges[0]);
            nodes[0].Edges.Add(edges[1]);
            nodes[0].Edges.Add(edges[3]);
            nodes[1].Edges.Add(edges[1]);
            nodes[1].Edges.Add(edges[2]);
            nodes[2].Edges.Add(edges[2]);
            nodes[2].Edges.Add(edges[3]);
            nodes[2].Edges.Add(edges[4]);
            nodes[2].Edges.Add(edges[11]);
            nodes[3].Edges.Add(edges[9]);
            nodes[3].Edges.Add(edges[10]);
            nodes[3].Edges.Add(edges[11]);
            nodes[4].Edges.Add(edges[5]);
            nodes[4].Edges.Add(edges[6]);
            nodes[5].Edges.Add(edges[6]);
            nodes[5].Edges.Add(edges[7]);
            nodes[6].Edges.Add(edges[7]);
            nodes[6].Edges.Add(edges[8]);
            nodes[6].Edges.Add(edges[10]);
            nodes[7].Edges.Add(edges[8]);
            nodes[7].Edges.Add(edges[9]);
            nodes[8].Edges.Add(edges[4]);
            nodes[8].Edges.Add(edges[5]);
            nodes[9].Edges.Add(edges[0]);

            edges[0].Node1 = nodes[0];
            edges[0].Node2 = nodes[9];
            edges[1].Node1 = nodes[0];
            edges[1].Node2 = nodes[1];
            edges[2].Node1 = nodes[1];
            edges[2].Node2 = nodes[2];
            edges[3].Node1 = nodes[0];
            edges[3].Node2 = nodes[2];
            edges[4].Node1 = nodes[2];
            edges[4].Node2 = nodes[8];
            edges[5].Node1 = nodes[4];
            edges[5].Node2 = nodes[8];
            edges[6].Node1 = nodes[4];
            edges[6].Node2 = nodes[5];
            edges[7].Node1 = nodes[5];
            edges[7].Node2 = nodes[6];
            edges[8].Node1 = nodes[6];
            edges[8].Node2 = nodes[7];
            edges[9].Node1 = nodes[3];
            edges[9].Node2 = nodes[7];
            edges[10].Node1 = nodes[3];
            edges[10].Node2 = nodes[6];
            edges[11].Node1 = nodes[2];
            edges[11].Node2 = nodes[3];

            var treeNodes = new TreeNode[10];
            for (int i = 0; i < treeNodes.Length; i++)
                treeNodes[i] = new TreeNode { Id = i, Depth = i };

            for (int i = 0; i < treeNodes.Length; i++)
            {
                var treeNode = treeNodes[i];
                if (i > 0)
                    treeNode.Parent = treeNodes[i - 1];
                if (i < treeNodes.Length - 1)
                    treeNode.Children.Add(treeNodes[i + 1]);
            }

            var maximumMatching = new MaximumMatchingTreedepth();
            var result = maximumMatching.Compute(nodes.ToList(), edges.ToList(), treeNodes[0], treeNodes.ToList());
            var matchedEdges = result.GetMatchedEdges();

            foreach (var edgeId in matchedEdges)
                Console.WriteLine(edgeId + 1);
            Console.WriteLine("Count: " + matchedEdges.Count);
        }


        private static void TestNegativeCycleDetection()
        {
            var nodes = new Node[3];
            for (int i = 0; i < nodes.Length; i++)
                nodes[i] = new Node { Id = i };
            var edges = new Edge[3];
            for (int i = 0; i < edges.Length; i++)
                edges[i] = new Edge { Id = i };

            nodes[0].Edges.Add(edges[0]);
            nodes[1].Edges.Add(edges[0]);
            edges[0].Node1 = nodes[0];
            edges[0].Node2 = nodes[1];
            edges[0].Weight = 12;

            nodes[1].Edges.Add(edges[1]);
            nodes[2].Edges.Add(edges[1]);
            edges[1].Node1 = nodes[1];
            edges[1].Node2 = nodes[2];
            edges[1].Weight = 8;

            nodes[2].Edges.Add(edges[2]);
            nodes[0].Edges.Add(edges[2]);
            edges[2].Node1 = nodes[2];
            edges[2].Node2 = nodes[0];
            edges[2].Weight = 22;

            var treeNodes = new TreeNode[3];
            for (int i = 0; i < treeNodes.Length; i++)
                treeNodes[i] = new TreeNode { Id = i, Depth = i };

            for (int i = 0; i < treeNodes.Length; i++)
            {
                var treeNode = treeNodes[i];
                if (i > 0)
                    treeNode.Parent = treeNodes[i - 1];
                if (i < treeNodes.Length - 1)
                    treeNode.Children.Add(treeNodes[i + 1]);
            }

            var negativeCycleDetectionTreedepth = new NegativeCycleDetectionTreedepth();
            var result = negativeCycleDetectionTreedepth.Compute(nodes.ToList(), edges.ToList(), treeNodes[0], treeNodes.ToList());

            Console.WriteLine("Treedepth:");
            if (result.NegativeCycleDetected)
                Console.WriteLine("Negative cycle detected");
            else
                Console.WriteLine("No negative cycle detected");
            Console.WriteLine();

            
            Console.WriteLine("Treewidth:");
            var negativeCycleDetectionTreewidth = new DirectedPathConsistency();
            var consitant = negativeCycleDetectionTreewidth.CheckConsistency(nodes, edges, new List<int>() { 0, 1, 2 });
            if (consitant)
                Console.WriteLine("No Negative cycle detected");
            else
                Console.WriteLine("negative cycle detected");
        }

        public static void TestTwoHopCover()
        {
            var circleSize = 10;

            var nodes = new Node[circleSize];
            var edges = new Edge[circleSize];
            for (int i = 0; i < circleSize; i++)
            {
                nodes[i] = new Node() { Id = i };
                edges[i] = new Edge() { Id = i, Weight = 1 };
            }

            for (int i = 0; i < circleSize; i++)
            {
                nodes[i].Edges.Add(edges[i]);
                nodes[i].Edges.Add(edges[(i - 1 + circleSize) % circleSize]);
                edges[i].Node1 = nodes[i];
                edges[i].Node2 = nodes[(i + 1) % circleSize];
            }

            var treeNodes = new TreeNode[circleSize];
            for (int i = 0; i < treeNodes.Length; i++)
                treeNodes[i] = new TreeNode { Id = i, Depth = i };

            for (int i = 0; i < treeNodes.Length; i++)
            {
                var treeNode = treeNodes[i];
                if (i > 0)
                    treeNode.Parent = treeNodes[i - 1];
                if (i < treeNodes.Length - 1)
                    treeNode.Children.Add(treeNodes[i + 1]);
            }

            var THC = new TwoHopCoverTreedepth();
            var index = THC.Compute(nodes.ToList(), edges.ToList(), treeNodes[0], treeNodes.ToList());
            var path = index.Query(2, 8);

            Console.WriteLine("Treedepth");
            Console.WriteLine("path length: " + (path.Length - 1));
            foreach (var p in path)
                Console.WriteLine(p);
            Console.WriteLine();

            var enhancedIndex = index.GetEnhancedResult();
            path = enhancedIndex.Query(2, 8);
            Console.WriteLine("Treedepth enhanced");
            Console.WriteLine("path length: " + (path.Length - 1));
            foreach (var p in path)
                Console.WriteLine(p);
            Console.WriteLine();

            var order = treeNodes[0].GetEliminationOrder();
            order.Reverse();
            var index2 = new TwoHopCoverTreewidth().ComputeIndex(nodes, edges, order);
            path = index2.Query(2, 8);
            Console.WriteLine("Treewidth");
            Console.WriteLine("path length: " + path.Length);
            foreach (var p in path)
                Console.WriteLine(p);

        }

        public static void TestLowestCommonAncestor()
        {
            var n0 = new TreeNode() { Id = 0, Depth = 0 };
            var n1 = new TreeNode() { Id = 1, Depth = 1 };
            var n2 = new TreeNode() { Id = 2, Depth = 1 };
            var n3 = new TreeNode() { Id = 3, Depth = 2 };
            var n4 = new TreeNode() { Id = 4, Depth = 2 };
            var n5 = new TreeNode() { Id = 5, Depth = 2 };
            var n6 = new TreeNode() { Id = 6, Depth = 3 };

            n0.Children.Add(n1);
            n0.Children.Add(n2);
            n1.Children.Add(n3);
            n2.Children.Add(n4);
            n2.Children.Add(n5);
            n4.Children.Add(n6);

            var lca = new LCA<TreeNode>(n0);
            var result = lca.Query(n6, n5);
            Console.WriteLine(result.Id);
            Console.ReadLine();
        }

        public static void DoWork()
        {
            var number = Console.ReadLine();
            number = number.PadLeft(3, '0');

            string filename = Environment.CurrentDirectory + "\\exact\\exact_" + number + ".gr";
            BasicTreedepthSolver.Graph graph;
            BasicTreedepthSolver.PartialSolution solution;

            using (StreamReader reader = File.OpenText(filename))
            {
                graph = BasicTreedepthSolver.Graph.ParsePACE2016(reader);
            }

            var graphEdges = graph.Edges
                .Select(e => new Tuple<string, string>(
                    (e.From.Id + 1).ToString() + " ",
                    (e.To.Id + 1).ToString() + " "
                    ))
                .ToList();

            solution = graph?.Treedepth();

            if (graph == null || solution == null)
            {
                Console.WriteLine("All has failed.");
                Console.ReadLine();
                return;
            }

            var solutionEdges = new List<Tuple<string, string>>();
            for (int i = 0; i < graph.Vertices.Length; i++)
            {
                if (solution.SolutionPrint[i] != "0")
                    solutionEdges.Add(new Tuple<string, string>(

                        solution.SolutionPrint[i],
                        (i + 1).ToString()
                        ));
            }

            var eliminationTree = new Tree(graph.Vertices.Length, solutionEdges.Select(t => new Tuple<int, int>(int.Parse(t.Item1) - 1, int.Parse(t.Item2) - 1)).ToArray());

            var graphLoader = new GraphLoader();
            Node[] nodes;
            Edge[] edges;
            graphLoader.LoadGraph(graph.Vertices.Length, graphEdges.Select(t => new Tuple<int, int>(int.Parse(t.Item1) - 1, int.Parse(t.Item2) - 1)).ToArray(), out nodes, out edges);


            var stopwatch = new Stopwatch();

            var twoHopCoverTreewidth = new TwoHopCoverTreewidth();
            var index1 = twoHopCoverTreewidth.ComputeIndex(nodes, edges, eliminationTree.Root.GetEliminationOrder());

            var twoHopCoverTreedepth = new TwoHopCoverTreedepth();
            var index2 = twoHopCoverTreedepth.Compute(nodes, edges, eliminationTree.Root, eliminationTree.Nodes);
            var index3 = index2.GetEnhancedResult();

            var times = new List<double>();
            var random = new Random();

            var size = 1000000;
            var queries = new int[size, 2];
            var answers = new int[size][];

            Console.WriteLine();
            
            Console.WriteLine("Queries:");

            
            for (var i = 0; i < size; i++)
            {
                var n1 = random.Next(nodes.Length);
                var n2 = random.Next(nodes.Length);
                queries[i, 0] = n1;
                queries[i, 1] = n2;
            }

            Console.WriteLine();
            Console.WriteLine("Treewidth:");
            stopwatch.Start();
            for (var i = 0; i < size; i++)
            {
                var n1 = queries[i, 0];
                var n2 = queries[i, 1];
                var answer = index1.Query(n1, n2);
                answers[i] = answer;
            }
            stopwatch.Stop();
            times.Add(stopwatch.Elapsed.TotalMilliseconds);
            stopwatch.Reset();

            foreach (var answer in answers)
                for (var i = 0; i < answer.Length; i++)
                    answer[i] = answer[i] + 1;

            //foreach (var answer in answers)
            //    Console.WriteLine(string.Join(',', answer));

            Console.WriteLine();
            Console.WriteLine("Treewidth:");
            stopwatch.Start();
            for (var i = 0; i < size; i++)
            {
                var n1 = queries[i, 0];
                var n2 = queries[i, 1];
                var answer = index2.Query(n1, n2);
                answers[i] = answer;
            }
            stopwatch.Stop();
            times.Add(stopwatch.Elapsed.TotalMilliseconds);
            stopwatch.Reset();

            foreach (var answer in answers)
                for (var i = 0; i < answer.Length; i++)
                    answer[i] = answer[i] + 1;

            //foreach (var answer in answers)
            //    Console.WriteLine(string.Join(',', answer));

            Console.WriteLine();
            Console.WriteLine("Treewidth enhanced:");
            stopwatch.Start();
            for (var i = 0; i < size; i++)
            {
                var n1 = queries[i, 0];
                var n2 = queries[i, 1];
                var answer = index3.Query(n1, n2);
                answers[i] = answer;
            }
            stopwatch.Stop();
            times.Add(stopwatch.Elapsed.TotalMilliseconds);
            stopwatch.Reset();

            foreach (var answer in answers)
                for (var i = 0; i < answer.Length; i++)
                    answer[i] = answer[i] + 1;

            //foreach (var answer in answers)
            //    Console.WriteLine(string.Join(',', answer));

            Console.WriteLine();
            Console.WriteLine("Times");
            for (var i = 0; i < times.Count; i += 3)
            {
                Console.WriteLine(times[i]);
                Console.WriteLine(times[i + 1]);
                Console.WriteLine(times[i + 2]);
                Console.WriteLine();
            }
        }
    }
}
