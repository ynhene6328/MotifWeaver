// /src/MotifWeaver.Core/Topology/VertexKey.cs
using System;

namespace MotifWeaver.Core.Topology;

public readonly struct VertexKey : IEquatable<VertexKey>
{
    public VertexKey(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }

    public int Y { get; }

    public bool Equals(VertexKey other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is VertexKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(VertexKey left, VertexKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VertexKey left, VertexKey right)
    {
        return !left.Equals(right);
    }
}
