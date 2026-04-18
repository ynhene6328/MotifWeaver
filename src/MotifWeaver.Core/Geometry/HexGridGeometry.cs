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
    private readonly RegularGridGeometry _inner;

    public HexGridGeometry(float unitSize)
    {
        _inner = new RegularGridGeometry(unitSize);
    }

    public Vector2 GetPosition(VertexKey key) => _inner.GetPosition(key);
    public BoundingBox ComputeBounds(IEnumerable<VertexKey> keys) => _inner.ComputeBounds(keys);
}