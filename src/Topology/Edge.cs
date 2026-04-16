// /src/Topology/Edge.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Topology;

public sealed class Edge
{
    private readonly List<Face> _faces;

    internal Edge(Vertex v1, Vertex v2)
    {
        if (v1 is null)
        {
            throw new ArgumentNullException(nameof(v1));
        }

        if (v2 is null)
        {
            throw new ArgumentNullException(nameof(v2));
        }

        if (ReferenceEquals(v1, v2))
        {
            throw new ArgumentException("Edge requires two distinct vertices.", nameof(v2));
        }

        V1 = v1;
        V2 = v2;
        Key = new EdgeKey(v1.Key, v2.Key);
        _faces = new List<Face>(2);
    }

    public EdgeKey Key { get; }

    public Vertex V1 { get; }

    public Vertex V2 { get; }

    public IReadOnlyList<Face> Faces => _faces;

    internal void AttachFace(Face face)
    {
        if (face is null)
        {
            throw new ArgumentNullException(nameof(face));
        }

        if (_faces.Contains(face))
        {
            return;
        }

        if (_faces.Count >= 2)
        {
            throw new InvalidOperationException("An edge cannot reference more than two faces.");
        }

        _faces.Add(face);
    }
}
