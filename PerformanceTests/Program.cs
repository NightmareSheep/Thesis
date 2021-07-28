using System;
using System.IO;
using Thesis;
using CsvHelper;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Thesis.TwoHopCover;
using Thesis.TwoHopCover.Treedepth;
using Thesis.TwoHopCover.Treewidth;
using Thesis.Graph;
using System.Diagnostics;
using Dijkstra;


namespace PerformanceTests
{
    class Program
    {

        static void Main(string[] args)
        {
            var negativeCycleMeasuring = new NegativeCycleMeasuring();

            negativeCycleMeasuring.RunMeasurements();
            //GenerateEliminationTrees(33);
            //Console.WriteLine(GetGraphResultsGraph(1));
            //GenerateResults();
            //GetEmptyResult();
            Console.ReadLine();
        }

        private static void GenerateEliminationTrees(int start)
        {
            var graphNr = start;
            while (true)
            {
                var number = graphNr.ToString("D3");
                var filename = "exact\\exact_" + number + ".gr";
                BasicTreedepthSolver.Graph graph;
                BasicTreedepthSolver.PartialSolution solution;

                if (!File.Exists(filename))
                    break;

                using (StreamReader reader = File.OpenText(filename))
                {
                    graph = BasicTreedepthSolver.Graph.ParsePACE2016(reader);
                }
                solution = graph?.Treedepth();

                //before your loop
                var csv = new StringBuilder();

                csv.AppendLine(graph.Vertices.Length.ToString());

                foreach (var line in solution.SolutionPrint)
                {
                    csv.AppendLine(line);
                }

                //after your loop
                Directory.CreateDirectory("EliminationTrees");
                File.WriteAllText("EliminationTrees\\EliminationTree" + graphNr + ".csv", csv.ToString());
                graphNr++;
            }
        }

        public static void GenerateResults()
        {
            var lines = new List<string>();
            for (var i = 1; i <= 120; i++)
            {
                lines.Add(GetGraphResults(i));
                Console.WriteLine(i);
            }

            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            var csv = new StringBuilder();

            csv.AppendLine("graphNumber,type,nrOfNodes,nrOfEdges,dijkstraTime,treewidthTime,treedepthTime,enhancedTime,enhancedTime+,treedepth, twPreTime, tdPreTime, td+PreTime, td++Pretime, twSize, tdSize, td+Size");

            foreach (var line in lines)
            {
                csv.AppendLine(line);
            }
            File.WriteAllText(timeStamp + ".csv", csv.ToString());
        }

        public static string GetGraphResults(int graphNr)
        {
            var graphLoader = new GraphLoader();
            graphLoader.LoadGraph(graphNr, out var nodes, out var edges, out var type);

            var eliminationTreeLoader = new EliminationTreeLoader();
            var eliminationTree = eliminationTreeLoader.LoadEliminationTree(graphNr, nodes.Length);

            //var twoHopCoverTreewidth = new TwoHopCoverTreewidth();
            //PerformanceTestIndex(() => { return twoHopCoverTreewidth.ComputeIndex(nodes, edges, eliminationTree.Root.GetEliminationOrder()); }, nodes.Length, 
            //    out var treewidthPrecomputationTime, out var treewidthSize, out var treewidthQueryTime);
            var treewidthQueryTime = 0;
            var treewidthPrecomputationTime = 0;
            var treewidthSize = 0;


            var twoHopCoverTreedepth = new TwoHopCoverTreedepth();
            PerformanceTestIndex(() => { return twoHopCoverTreedepth.Compute(nodes, edges, eliminationTree.Root, eliminationTree.Nodes); }, nodes.Length,
                out var treedepthPrecomputationTime, out var treedepthSize, out var treedepthQueryTime);

            PerformanceTestIndex(() => { return twoHopCoverTreedepth.Compute(nodes, edges, eliminationTree.Root, eliminationTree.Nodes).GetEnhancedResult3(); }, nodes.Length,
                out var enhanced2TreedepthPrecomputationTime, out var enhanced2TreedepthSize, out var enhanced2TreedepthQueryTime);

            PerformanceTestIndex(() => { return twoHopCoverTreedepth.Compute(nodes, edges, eliminationTree.Root, eliminationTree.Nodes).GetEnhancedResult2(); }, nodes.Length,
                out var enhancedTreedepthPrecomputationTime, out var enhancedTreedepthSize, out var enhancedTreedepthQueryTime);


            // {graphNumber};{type};{nrOfNodes};{nrOfEdges};{dijkstraTime};{treewidthTime};{treedepthTime};{treedepthTime+};{treedepthEnhancedTime++};{treedepth};{treewidthPrecomputation};{treedepthPrecomputation+};{treewidth+Precomputation++};{treewidthSize};{treedepthSize};{treedepth+Size}
            var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}",
                graphNr, type, nodes.Length, edges.Length, 
                0, treewidthQueryTime, treedepthQueryTime, enhanced2TreedepthQueryTime, enhancedTreedepthQueryTime, 
                eliminationTree.Depth, 
                treewidthPrecomputationTime, treedepthPrecomputationTime, enhanced2TreedepthPrecomputationTime, enhancedTreedepthPrecomputationTime, 
                treewidthSize, treedepthSize, enhancedTreedepthSize);

            return line;
        }

        /// <summary>
        /// Test performance of the index. Returns precomputation time, query time and index size.
        /// </summary>
        /// <param name="BuildIndex"></param>
        /// <param name="nrOfNodes"></param>
        /// <param name="precomputationTime"></param>
        /// <param name="size"></param>
        /// <param name="queryTime"></param>
        private static void PerformanceTestIndex(Func<IIndex> BuildIndex, int nrOfNodes, out long precomputationTime, out int size, out long queryTime)
        {
            var indexIterations = 100;
            var nrOfQueries = 100000;

            var stopwatch = new Stopwatch();

            // Measure precomputation time
            var runs = new long[indexIterations];
            for (var i = 0; i < indexIterations; i++)
            {
                stopwatch.Start();
                BuildIndex();
                stopwatch.Stop();
                runs[i] = stopwatch.ElapsedMilliseconds;
                if (runs[i] > 10000)
                {
                    for (var j = i; j < indexIterations; j++)
                        runs[j] = runs[i];
                    i = indexIterations;
                }
                stopwatch.Reset();
            }
            Array.Sort(runs);
            precomputationTime = runs[runs.Length / 2];

            // Measure size
            var index = BuildIndex();
            size = index.Size;

            // Measure query time
            var random = new Random();
            stopwatch.Start();
            for (int i = 0; i < nrOfQueries; i++)
            {
                var v = random.Next(nrOfNodes);
                var w = random.Next(nrOfNodes);
                index.Query(v, w);
            }
            stopwatch.Stop();
            queryTime = stopwatch.ElapsedMilliseconds;
        }

        public static void GetEmptyResult()
        {
            var lines = new List<string>();
            for (var i = 1; i <= 200; i++)
            {
                var number = i.ToString("D3");
                var filename = "heur\\heur_" + number + ".gr";
                var type = "";
                var nrOfNodes = 0;
                var nrOfEdges = 0;
                using (StreamReader tr = File.OpenText(filename))
                {
                    tr.ReadLine();
                    type = tr.ReadLine().Substring(8).Trim();

                    string readline;
                    while ((readline = tr.ReadLine())[0] != 'p') { }
                    var words = readline.Split(' ');
                    nrOfNodes = int.Parse(words[2]);
                    nrOfEdges = int.Parse(words[3]);
                }

                var line = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                i, type, nrOfNodes, nrOfEdges, 0, 0, 0, 0, 0);
                lines.Add(line);
            }

            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            var csv = new StringBuilder();

            csv.AppendLine("graphNumber,type,nrOfNodes,nrOfEdges,dijkstraTime,treewidthTime,treedepthTime,enhancedTime,enhancedTime+, treedepth");

            foreach (var line in lines)
            {
                csv.AppendLine(line);
            }
            File.WriteAllText(timeStamp + ".csv", csv.ToString());
        }
    }
}
