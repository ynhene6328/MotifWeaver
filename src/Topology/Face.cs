// /src/Topology/Face.cs
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MotifWeaver.Topology;

public sealed class Face
{
    private readonly List<Vertex> _vertices;
    private readonly List<Edge> _edges;

    internal Face(IReadOnlyList<Vertex> vertices, IReadOnlyList<Edge> edges)
    {
        if (vertices is null)
        {
            throw new ArgumentNullException(nameof(vertices));
        }

        if (edges is null)
        {
            throw new ArgumentNullException(nameof(edges));
        }

        if (vertices.Count < 3)
        {
            throw new ArgumentException("A face requires at least three vertices.", nameof(vertices));
        }

        if (vertices.Count != edges.Count)
        {
            throw new ArgumentException("Vertex and edge counts must match.", nameof(edges));
        }

        _vertices = new List<Vertex>(vertices.Count);
        _edges = new List<Edge>(edges.Count);

        for (int index = 0; index < vertices.Count; index++)
        {
            _vertices.Add(vertices[index]);
            _edges.Add(edges[index]);
        }
    }

    public IReadOnlyList<Vertex> Vertices => _vertices;

    public IReadOnlyList<Edge> Edges => _edges;

    public Color Color { get; set; } = Color.White;
}
