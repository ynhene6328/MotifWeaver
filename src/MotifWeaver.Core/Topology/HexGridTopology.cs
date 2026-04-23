// /src/MotifWeaver.Core/Topology/HexGridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class HexGridTopology : GridTopology
{
    public override int UnitX => 3;
    public override int UnitY => 2;
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

    private static IReadOnlyList<VertexKey> CreateHexVertices(int q, int r)
    {
        int cx = 3 * q;
        int cy = r * 2;

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
    public override int CalculateLogicalWidth()
    {
        var targetVertices = new List<VertexKey>();
        foreach(var face in _topologyBuilder.Faces)
        {
            targetVertices.AddRange(face.Vertices.Where(v => v.Key.Y == 1).Select(v => v.Key));
        }
        var maxX = targetVertices.Max(v => v.X);
        var minX = targetVertices.Min(v => v.X);
        
        return maxX - minX;
    }

    public override int CalculateLogicalHeight()
    {
        var maxY = _topologyBuilder.Faces.Max(f => f.Vertices.Max(v => v.Key.Y));
        return maxY - 1;
    }
    public override IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0)
    {
        if (cols % 2 != 0)
            throw new ArgumentException("列数は偶数である必要があります");

        return ResizeCore(rows, cols, baseCol * 3, baseRow * 2);
    }
}
