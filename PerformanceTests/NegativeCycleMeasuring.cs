using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Thesis.Graph;
using Thesis.NegativeCycleDetection;

namespace PerformanceTests
{
    public class NegativeCycleMeasuring
    {
        public void RunMeasurements()
        {
            var lines = new List<string>();
            for (var i = 42; i <= 42; i++)
            {
                lines.Add(GetGraphResults(i));
                Console.WriteLine(i);
            }

            var timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            var csv = new StringBuilder();

            csv.AppendLine("graphNumber,type,nrOfNodes,nrOfEdges,treewidthTime,treedepthTime");

            foreach (var line in lines)
            {
                csv.AppendLine(line);
            }
            File.WriteAllText("negativeCycleDetection" + timeStamp + ".csv", csv.ToString());
        }

        private string GetGraphResults(int graphNr)
        {
            var graphLoader = new GraphLoader();
            graphLoader.LoadGraph(graphNr, out var nodes, out var edges, out var type);

            var eliminationTreeLoader = new EliminationTreeLoader();
            var eliminationTree = eliminationTreeLoader.LoadEliminationTree(graphNr, nodes.Length);

            long treewidthTime = 0;
            long treedepthTime = 0;

            var random = new Random();


            for (var i = 0; i < 5; i++)
            {
                foreach (var edge in edges)
                {
                    edge.Weight = random.Next(10) - i;
                }

                var treewidthResult = MeasureOnGraph(() => { return !(new DirectedPathConsistency().CheckConsistency(nodes, edges, eliminationTree.Root)); }, out var treewidthRunTime);

                var treedepthResult = MeasureOnGraph(() => { return new NegativeCycleDetectionTreedepth().Compute(nodes, edges, eliminationTree.Root, eliminationTree.Nodes).NegativeCycleDetected; }, out var treedepthRunTime);

                if (treewidthResult != treedepthResult)
                    throw new Exception("Inconsistent results on same graph");

                treewidthTime += treewidthRunTime;
                treedepthTime += treedepthRunTime;
            }

            var line = string.Format("{0},{1},{2},{3},{4},{5}",
                graphNr, type, nodes.Length, edges.Length,
                treewidthTime, treedepthTime);
            return line;
        }

        private bool MeasureOnGraph(Func<bool> DetectNegativeCycle, out long time)
        {
            time = 0;
            var stopwatch = new Stopwatch();
            var iterations = 10000;

            var currentResult = false;
            var previousResult = false;

            stopwatch.Start();
            for (var i = 0; i < iterations; i++)
            {
                currentResult = DetectNegativeCycle();
                if (i != 0 && currentResult != previousResult)
                    throw new Exception("Inconsistent results on the same graph");
                previousResult = currentResult;
            }
            stopwatch.Stop();

            time = stopwatch.ElapsedMilliseconds;
            return currentResult;
        }
    }
}
