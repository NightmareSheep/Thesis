using System;
using System.Collections.Generic;
using System.Text;

namespace Thesis.MaximumMatching
{
    public class Entry
    {
        public int Mate { get; set; } = -1;
        public int First { get; set; } = -1;
        public LabelType LabelType { get; set; }
        public int Vertex { get; set; }
        public int vertexLabelEdge { get; set; } = -1;
        public int Edge { get; set; } = -1;
       
    }
}
