// /src/Topology/EdgeKey.cs
using System;

namespace MotifWeaver.Topology;

public readonly struct EdgeKey : IEquatable<EdgeKey>
{
    public EdgeKey(VertexKey first, VertexKey second)
    {
        if (first == second)
        {
            throw new ArgumentException("Edge requires two distinct vertices.", nameof(second));
        }

        if (Compare(first, second) <= 0)
        {
            A = first;
            B = second;
        }
        else
        {
            A = second;
            B = first;
        }
    }

    public VertexKey A { get; }

    public VertexKey B { get; }

    public bool Equals(EdgeKey other)
    {
        return A == other.A && B == other.B;
    }

    public override bool Equals(object? obj)
    {
        return obj is EdgeKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(A, B);
    }

    public static bool operator ==(EdgeKey left, EdgeKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EdgeKey left, EdgeKey right)
    {
        return !left.Equals(right);
    }

    private static int Compare(VertexKey left, VertexKey right)
    {
        int compareX = left.X.CompareTo(right.X);
        if (compareX != 0)
        {
            return compareX;
        }

        return left.Y.CompareTo(right.Y);
    }
}
