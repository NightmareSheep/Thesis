using System;
using System.Collections.Generic;
using System.Text;

namespace Thesis
{
    public interface IIndex
    {
        int[] Query(int v, int w);
        int Size { get; }
    }
}
