// /src/MotifWeaver.Core/Topology/FaceKey.cs
using System;

namespace MotifWeaver.Core.Topology;

public readonly struct GridFaceKey : IEquatable<GridFaceKey>
{
    VertexKey Anchor { get; }
    public GridFaceKey(Face face)
    {
        Anchor = GetFaceAnchor(face);
    }
    private static VertexKey GetFaceAnchor(Face face)
    {
        return face.Vertices
            .Select(v => v.Key)
            .OrderBy(v => v.X)
            .ThenBy(v => v.Y)
            .First();
    }
    public bool Equals(GridFaceKey other)
    {
        return Anchor == other.Anchor;
    }
    public override bool Equals(object? obj)
    {
        return obj is GridFaceKey other && Equals(other);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Anchor);
    }
    public static bool operator ==(GridFaceKey left, GridFaceKey right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(GridFaceKey left, GridFaceKey right)
    {
        return !left.Equals(right);
    }
}