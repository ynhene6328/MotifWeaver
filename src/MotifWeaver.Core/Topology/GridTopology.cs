// /src/MotifWeaver.Core/Topology/GridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public abstract class GridTopology : IGridTopology
{
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

    public abstract (int width, int height) CalculateSize(IReadOnlyList<Face> faces);

    public abstract IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0);

    protected IReadOnlyList<Face> ResizeCore(int rows, int cols, int offsetX, int offsetY)
    {
        var newTopology = new HexGridTopology();
        var newFaces = newTopology.Build(rows, cols);

        _topologyBuilder.Offset(offsetX, offsetY);

        newTopology.Builder.CopyAttributesFrom(_topologyBuilder, faceComparator: (f1, f2) =>
        {
            var k1 = new GridFaceKey(f1);
            var k2 = new GridFaceKey(f2);
            return k1.Equals(k2);
        });

        _topologyBuilder = newTopology.Builder;
        return newFaces;
    }
    
}
