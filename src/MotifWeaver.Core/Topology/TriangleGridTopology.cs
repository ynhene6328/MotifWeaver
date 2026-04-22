// /src/MotifWeaver.Core/Topology/TriangleGridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class TriangleGridTopology : GridTopology
{
    public override IReadOnlyList<Face> Build(int rows, int cols)
    {
        if (rows < 0)
            throw new ArgumentOutOfRangeException(nameof(rows));

        if (cols < 0)
            throw new ArgumentOutOfRangeException(nameof(cols));

        List<Face> faces = new List<Face>(rows * cols * 2);

        for (int r = 0; r < rows; r++)
        {
            for (int q = 0; q < cols; q++)
            {
                int cx = 2 * q;
                int cy = r;

                // 上向き三角形
                cx += (r % 2 == 0) ? 0 : 1;
                faces.Add(_topologyBuilder.CreateFace(new[]
                {
                    new VertexKey(cx,     cy),
                    new VertexKey(cx + 2, cy),
                    new VertexKey(cx + 1, cy + 1)
                }));

                // 下向き三角形
                cx += (r % 2 == 0) ? 1 : -1;
                faces.Add(_topologyBuilder.CreateFace(new[]
                {
                    new VertexKey(cx, cy + 1),
                    new VertexKey(cx + 1, cy),
                    new VertexKey(cx + 2, cy + 1)
                }));
            }
        }

        return faces;
    }

    public override (int width, int height) CalculateSize(IReadOnlyList<Face> faces)
    {
        var topVertices = new List<VertexKey>();
        foreach(var face in faces)
        {
            topVertices.AddRange(face.Vertices.Where(v => v.Key.Y == 0).Select(v => v.Key));
        }
        var maxX = topVertices.Max(v => v.X);
        var minX = topVertices.Min(v => v.X);
        
        var maxY = faces.Max(f => f.Vertices.Max(v => v.Key.Y));

        return (maxX - minX, maxY);
    }

    public override IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0)
    {
        if (cols % 2 != 0)
            throw new ArgumentException("列数は偶数である必要があります");

        return ResizeCore(rows, cols, baseCol * 2, baseRow);
    }
}