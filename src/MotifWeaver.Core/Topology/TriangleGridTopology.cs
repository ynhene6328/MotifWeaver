// /src/MotifWeaver.Core/Topology/TriangleGridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class TriangleGridTopology : GridTopology
{
    public override int UnitX => 2;
    public override int UnitY => 1;
    public override int LogicalWidth => MaxWidth - 1;
    public override int LogicalHeight => MaxHeight;
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

    public override IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0)
    {
        if (cols % 2 != 0)
            throw new ArgumentException("列数は偶数である必要があります");

        return ResizeCore(new TriangleGridTopology(), rows, cols, baseCol * 2, baseRow);
    }
}