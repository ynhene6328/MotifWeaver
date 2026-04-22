// /src/MotifWeaver.Core/Topology/FaceKey.cs
using System;

namespace MotifWeaver.Core.Topology;

public readonly struct FaceKey : IEquatable<FaceKey>
{
    VertexKey Anchor { get; }
    public FaceKey(IReadOnlyList<Vertex> vertices)
    {
        if (vertices is null)
        {
            throw new ArgumentNullException(nameof(vertices));
        }

        if (vertices.Count < 3)
        {
            throw new ArgumentException("A face requires at least three vertices.", nameof(vertices));
        }

        Anchor = GetFaceAnchor(vertices);
    }
    private static VertexKey GetFaceAnchor(IReadOnlyList<Vertex> vertices)
    {
        return vertices
            .Select(v => v.Key)
            .OrderBy(v => v.X)
            .ThenBy(v => v.Y)
            .First();
    }
    public bool Equals(FaceKey other)
    {
        return Anchor == other.Anchor;
    }
    public override bool Equals(object? obj)
    {
        return obj is FaceKey other && Equals(other);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Anchor);
    }
    public static bool operator ==(FaceKey left, FaceKey right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(FaceKey left, FaceKey right)
    {
        return !left.Equals(right);
    }
}