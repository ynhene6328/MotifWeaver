// /src/MotifWeaver.Core/Topology/HexGridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class HexGridTopology : GridTopology
{
    public override int UnitX => 3;
    public override int UnitY => 2;
    public override int UnitRow => 1;
    public override int UnitCol => 2;
    public override int LogicalWidth => MaxWidth - 1;
    public override int LogicalHeight => MaxHeight - 1;
    protected override IReadOnlyList<VertexKey> CreateGridFaceVertices(int row, int col)
    {
        int cx = UnitX * col;
        int cy = row * UnitY;

        cy += (col % 2 == 0) ? 0 : 1;

        return
        [
            new VertexKey(cx + 1, cy),
            new VertexKey(cx + 3, cy),
            new VertexKey(cx + 4, cy + 1),
            new VertexKey(cx + 3, cy + 2),
            new VertexKey(cx + 1, cy + 2),
            new VertexKey(cx    , cy + 1)
        ];
    }
    public override (int row, int col) GetGridFaceRowCol(Face face)
    {
        var baseX = face.Vertices.Min(v => v.Key.X);
        var baseY = face.Vertices.Min(v => v.Key.Y);

        int col = baseX / UnitX;
        int row = (baseY - (col % 2 == 0 ?  0 : 1)) / UnitY;

        return (row, col);
    }

    public override IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0)
    {
        if (cols % 2 != 0)
            throw new ArgumentException("列数は偶数である必要があります");

        return ResizeCore(new HexGridTopology(), rows, cols, baseRow, baseCol);
    }
}
