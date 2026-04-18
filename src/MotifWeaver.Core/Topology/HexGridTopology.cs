// /src/MotifWeaver.Core/Topology/HexGridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class HexGridTopology
{
    private readonly TopologyBuilder _topologyBuilder;

    public HexGridTopology()
        : this(new TopologyBuilder())
    {
    }

    public HexGridTopology(TopologyBuilder topologyBuilder)
    {
        _topologyBuilder = topologyBuilder ?? throw new ArgumentNullException(nameof(topologyBuilder));
    }

    public TopologyBuilder Builder => _topologyBuilder;

    public IReadOnlyList<Face> Build(int rows, int cols)
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
}
