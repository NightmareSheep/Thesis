using System;
using System.Collections.Generic;
using System.Text;

namespace Thesis.TwoHopCover
{
    public class Entry
    {
        public int To { get; set; }

        // Points to a vertex in the path.
        public int Middle { get; set; }
        public int Length { get; set; }
    }
}
