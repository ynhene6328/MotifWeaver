// /src/MotifWeaver.Core/Topology/GridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public abstract class GridTopology : IGridTopology
{
    public abstract int UnitX { get; }
    public abstract int UnitY { get; }
    public int MaxWidth => _topologyBuilder.Faces.Max(f => f.Vertices.Max(v => v.Key.X)) - _topologyBuilder.Faces.Min(f => f.Vertices.Min(v => v.Key.X));
    public int MaxHeight => _topologyBuilder.Faces.Max(f => f.Vertices.Max(v => v.Key.Y)) - _topologyBuilder.Faces.Min(f => f.Vertices.Min(v => v.Key.Y));
    public abstract int LogicalWidth { get; }
    public abstract int LogicalHeight { get; }
    public int Row => LogicalHeight / UnitY;
    public int Col => LogicalWidth / UnitX;

    protected TopologyBuilder _topologyBuilder;
    public GridTopology()
        : this(new TopologyBuilder())
    {
    }

    public GridTopology(TopologyBuilder topologyBuilder)
    {
        _topologyBuilder = topologyBuilder ?? throw new ArgumentNullException(nameof(topologyBuilder));
    }

    public TopologyBuilder Builder => _topologyBuilder;

    public abstract IReadOnlyList<Face> Build(int rows, int cols);
    public abstract IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0);

    protected IReadOnlyList<Face> ResizeCore(GridTopology newGridTopology, int rows, int cols, int offsetX, int offsetY)
    {
        var newFaces = newGridTopology.Build(rows, cols);

        _topologyBuilder.Offset(offsetX, offsetY);

        newGridTopology.Builder.CopyAttributesFrom(_topologyBuilder, faceComparator: (f1, f2) =>
        {
            var k1 = new GridFaceKey(f1);
            var k2 = new GridFaceKey(f2);
            return k1.Equals(k2);
        });

        _topologyBuilder = newGridTopology.Builder;
        return newFaces;
    }
    
}
