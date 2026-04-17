// /src/MotifWeaver.Core/Geometry/HexGridGeometry.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Core.Geometry;

/// <summary>
/// 六角格子の整数座標を描画座標に変換する
/// </summary>
public sealed class HexGridGeometry : IGridGeometry
{
    private readonly float _unitSize;
    private readonly Dictionary<VertexKey, Vector2> _positionCache;

    public HexGridGeometry(float unitSize)
    {
        if (unitSize <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(unitSize));
        }

        _unitSize = unitSize;
        _positionCache = new Dictionary<VertexKey, Vector2>();
    }

    public Vector2 GetPosition(VertexKey key)
    {
        if (_positionCache.TryGetValue(key, out Vector2 cached))
        {
            return cached;
        }

        // 設計書: x = X * (w / 2), y = Y * (√3 / 2) * w
        float x = key.X * (_unitSize / 2.0f);
        float y = key.Y * (MathF.Sqrt(3.0f) / 2.0f) * _unitSize;

        Vector2 position = new Vector2(x, y);
        _positionCache.Add(key, position);
        return position;
    }

    public BoundingBox ComputeBounds(IEnumerable<VertexKey> vertexKeys)
    {
        if (vertexKeys is null)
        {
            throw new ArgumentNullException(nameof(vertexKeys));
        }

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;
        bool hasAny = false;

        foreach (VertexKey key in vertexKeys)
        {
            Vector2 position = GetPosition(key);

            if (position.X < minX) minX = position.X;
            if (position.Y < minY) minY = position.Y;
            if (position.X > maxX) maxX = position.X;
            if (position.Y > maxY) maxY = position.Y;
            hasAny = true;
        }

        if (!hasAny)
        {
            throw new ArgumentException("At least one vertex key is required.", nameof(vertexKeys));
        }

        return new BoundingBox(minX, minY, maxX, maxY);
    }
}
