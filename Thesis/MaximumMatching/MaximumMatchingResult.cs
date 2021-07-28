using System;
using System.Collections.Generic;
using System.Text;

namespace Thesis.MaximumMatching
{
    public class MaximumMatchingResult
    {
        public List<int> GetMatchedEdges()
        {
            if (Entries == null)
                return null;

            var edges = new List<int>();
            foreach (var entry in Entries)
                if (entry.Edge != -1)
                    edges.Add(entry.Edge);
            return edges;
        }

        public List<int> UnMatchedVertexIds { get; set; } = new List<int>();
        public Entry[] Entries;
    }
}
