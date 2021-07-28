using System;
using System.Collections.Generic;
using System.Text;
using Thesis.Graph;

namespace Thesis.NegativeCycleDetection
{
    public class NegativeCycleDetectionResult
    {
        public bool NegativeCycleDetected { get; set; }
        public int[] NegativeCycle { get; set; }
    }
}
