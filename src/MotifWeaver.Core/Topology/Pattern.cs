using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class Pattern
{
    public IGridTopology Topology { get; }

    public IReadOnlyList<Face> Faces { get; private set; }

    public int Rows { get; private set; }
    public int Cols { get; private set; }

    public Pattern(IGridTopology topology, int rows, int cols)
    {
        if (topology is null)
            throw new ArgumentNullException(nameof(topology));
        
        if (cols % 2 != 0)
            throw new ArgumentException("列数は偶数である必要があります");

        Topology = topology;
        Rows = rows;
        Cols = cols;

        Faces = topology.Build(rows, cols);
    }
}
