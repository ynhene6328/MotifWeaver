// /src/MotifWeaver.Core/Topology/HexGridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class HexGridTopology : GridTopology
{
    public override int UnitX => 3;
    public override int UnitY => 2;
    public override int LogicalWidth => MaxWidth - 1;
    public override int LogicalHeight => MaxHeight - 1;
    public override IReadOnlyList<Face> Build(int rows, int cols)
    {
        if (rows < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows));
        }

        if (cols < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cols));
        }

        List<Face> faces = new List<Face>(rows * cols);

        for (int r = 0; r < rows; r++)
        {
            for (int q = 0; q < cols; q++)
            {
                IReadOnlyList<VertexKey> vertices = CreateHexVertices(q, r);
                Face face = _topologyBuilder.CreateFace(vertices);
                faces.Add(face);
            }
        }

        return faces;
    }

    private IReadOnlyList<VertexKey> CreateHexVertices(int q, int r)
    {
        int cx = UnitX * q;
        int cy = r * UnitY;

        cy += (q % 2 == 0) ? 0 : 1;

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

    public override IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0)
    {
        if (cols % 2 != 0)
            throw new ArgumentException("列数は偶数である必要があります");

        return ResizeCore(new HexGridTopology(), rows, cols, baseCol * UnitX, baseRow * UnitY);
    }
}
