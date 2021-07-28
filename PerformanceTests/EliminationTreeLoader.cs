using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Thesis.Trees;

namespace PerformanceTests
{
    public class EliminationTreeLoader
    {
        public Tree LoadEliminationTree(int graphNr, int nrOfNodes)
        {
            string filename = Environment.CurrentDirectory + "\\EliminationTreesHeur\\result" + graphNr.ToString("D3") + ".txt";
            List<Tuple<int, int>> ParentChildPairs;
            using (StreamReader reader = File.OpenText(filename))
            {
                reader.ReadLine();
                ParentChildPairs = new List<Tuple<int, int>>();
                var j = 0;
                for (var i = 0; i < nrOfNodes; i++)
                {
                    var parentId = int.Parse(reader.ReadLine());
                    if (parentId != 0)
                    {
                        ParentChildPairs.Add(new Tuple<int, int>(parentId - 1, i));
                        j++;
                    }
                }
            }
            return new Tree(nrOfNodes, ParentChildPairs.ToArray());
        }
    }
}
