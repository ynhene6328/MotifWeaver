// /src/MotifWeaver.Core/Topology/TriangleGridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class TriangleGridTopology
{
    private readonly TopologyBuilder _topologyBuilder;

    public TriangleGridTopology()
        : this(new TopologyBuilder())
    {
    }

    public TriangleGridTopology(TopologyBuilder topologyBuilder)
    {
        _topologyBuilder = topologyBuilder ?? throw new ArgumentNullException(nameof(topologyBuilder));
    }

    public TopologyBuilder Builder => _topologyBuilder;

    public IReadOnlyList<Face> Build(int rows, int cols)
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
}