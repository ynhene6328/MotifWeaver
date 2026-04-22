// /src/MotifWeaver.Core/Topology/VertexKey.cs
using System;

namespace MotifWeaver.Core.Topology;

public class VertexKey : IEquatable<VertexKey>
{
    public VertexKey(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; private set;}

    public int Y { get; private set;}
    public void Offset(int offsetX, int offsetY)
    {
        X += offsetX;
        Y += offsetY;
    }

    public bool Equals(VertexKey? other)
    {
        return other is not null && X == other.X && Y == other.Y;
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
