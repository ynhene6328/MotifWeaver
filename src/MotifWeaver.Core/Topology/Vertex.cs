// /src/MotifWeaver.Core/Topology/Vertex.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class Vertex
{
    private readonly List<Edge> _edges;

    internal Vertex(VertexKey key)
    {
        Key = key;
        _edges = new List<Edge>();
    }

    public VertexKey Key { get; }

    public IReadOnlyList<Edge> Edges => _edges;

    internal void AttachEdge(Edge edge)
    {
        if (edge is null)
        {
            throw new ArgumentNullException(nameof(edge));
        }

        if (_edges.Contains(edge))
        {
            return;
        }

        _edges.Add(edge);
    }
}
