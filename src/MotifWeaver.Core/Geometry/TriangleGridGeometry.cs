// /src/MotifWeaver.Core/Geometry/TriangleGridGeometry.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Core.Geometry;

/// <summary>
/// 三角格子（整数座標ベース）を描画座標に変換する
/// </summary>
public sealed class TriangleGridGeometry : IGridGeometry
{
    private readonly RegularGridGeometry _inner;

    public TriangleGridGeometry(float unitSize)
    {
        _inner = new RegularGridGeometry(unitSize);
    }

    public Vector2 GetPosition(VertexKey key) => _inner.GetPosition(key);
    public BoundingBox ComputeBounds(IEnumerable<VertexKey> keys) => _inner.ComputeBounds(keys);
    public Vector2 ToLogicalPosition(Vector2 screenPosition) => _inner.ToLogicalPosition(screenPosition);
}