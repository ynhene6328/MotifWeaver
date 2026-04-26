// /src/MotifWeaver.Core/Geometry/RegularGridGeometry.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Core.Geometry;

/// <summary>
/// 六角格子の整数座標を描画座標に変換する
/// </summary>
public sealed class RegularGridGeometry : IGridGeometry
{
    private readonly float _unitSize;
    private readonly Dictionary<VertexKey, Vector2> _positionCache;

    public RegularGridGeometry(float unitSize)
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

    public Vector2 ToLogicalPosition(Vector2 screenPosition)
    {
        float logicalX = screenPosition.X / ((1.0f / 2.0f) * _unitSize);
        float logicalY = screenPosition.Y / ((MathF.Sqrt(3.0f) / 2.0f) * _unitSize);
        return new Vector2(logicalX, logicalY);
    }

    // 点との距離が閾値内のEdgeを返す。複数ある場合は最も距離が近いものを返す。ない場合はnullを返す。
    public Edge? FindClosestEdge(Face face, Vector2 point, float threshold)
    {
        Edge? closestEdge = null;
        float minDistance = float.MaxValue;

        foreach (var edge in face.Edges)
        {
            var v1 = edge.V1.Key;
            var v2 = edge.V2.Key;

            Vector2 p1 = GetPosition(v1);
            Vector2 p2 = GetPosition(v2);

            float distanceToLine = DistancePointToLine(point, p1, p2);
            float distanceToV1 = Vector2.Distance(point, p1);
            float distanceToV2 = Vector2.Distance(point, p2);

            float distance = Math.Min(distanceToLine, Math.Min(distanceToV1, distanceToV2));

            if (distance < threshold && distance < minDistance)
            {
                minDistance = distance;
                closestEdge = edge;
            }
        }

        return closestEdge;
    }

    // 点と直線（2点）との距離
    static float DistancePointToLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 lineDirection = lineEnd - lineStart;
        float lineLengthSquared = lineDirection.LengthSquared();

        if (lineLengthSquared == 0.0f)
        {
            return Vector2.Distance(point, lineStart);
        }

        float t = Vector2.Dot(point - lineStart, lineDirection) / lineLengthSquared;
        t = Math.Clamp(t, 0.0f, 1.0f);

        Vector2 projection = lineStart + t * lineDirection;
        return Vector2.Distance(point, projection);
    }
}
