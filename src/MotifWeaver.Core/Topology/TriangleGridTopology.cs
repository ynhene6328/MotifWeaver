// /src/MotifWeaver.Core/Topology/TriangleGridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class TriangleGridTopology : GridTopology
{
    public override int UnitX => 1;
    public override int UnitY => 1;
    public override int UnitRow => 2;
    public override int UnitCol => 2;
    public override int LogicalWidth => MaxWidth - 1;
    public override int LogicalHeight => MaxHeight;
    protected override IReadOnlyList<VertexKey> CreateGridFaceVertices(int row, int col)
    {
        int cx = col;
        int cy = row;

        if ((col % 2 == 0 && row % 2 == 0) || (col % 2 == 1 && row % 2 == 1))
        {
            return new []{
                new VertexKey(cx,     cy),
                new VertexKey(cx + 2, cy),
                new VertexKey(cx + 1, cy + 1)
            };
        }
        else
        {
            return new[]
            {
                new VertexKey(cx, cy + 1),
                new VertexKey(cx + 1, cy),
                new VertexKey(cx + 2, cy + 1)
            };
        }        
    }
    public override (int row, int col) GetGridFaceRowCol(Face face)
    {
        var baseX = face.Vertices.Min(v => v.Key.X);
        var baseY = face.Vertices.Min(v => v.Key.Y);

        int col = baseX;
        int row = baseY;

        return (row, col);
    }

    public override IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0)
    {
        return ResizeCore(new TriangleGridTopology(), rows, cols, baseRow, baseCol);
    }
}