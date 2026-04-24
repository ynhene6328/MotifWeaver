using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class Pattern
{
    public IGridTopology Topology { get; }

    public IReadOnlyList<Face> Faces { get; private set; }

    public Pattern(IGridTopology topology, int rows, int cols)
    {
        if (topology is null)
            throw new ArgumentNullException(nameof(topology));
        
        Topology = topology;

        if (rows <= 0)
            throw new ArgumentOutOfRangeException(nameof(rows));

        if (cols <= 0)
            throw new ArgumentOutOfRangeException(nameof(cols));

        Faces = topology.Build(rows, cols);
    }

    public void Resize(int rows, int cols, int baseRow = 0, int baseCol = 0)
    {
        if (rows <= 0)
            throw new ArgumentOutOfRangeException(nameof(rows));

        if (cols <= 0)
            throw new ArgumentOutOfRangeException(nameof(cols));

        Faces = Topology.Resize(rows, cols, baseRow, baseCol);
    }
}
