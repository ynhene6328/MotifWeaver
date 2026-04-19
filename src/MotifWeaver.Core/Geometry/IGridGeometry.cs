// /src/MotifWeaver.Core/Geometry/IGridGeometry.cs
using System.Collections.Generic;
using System.Numerics;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Core.Geometry;

/// <summary>
/// 整数座標から描画座標への変換を定義するインターフェース
/// </summary>
public interface IGridGeometry
{
    /// <summary>
    /// 整数座標を描画座標に変換する
    /// </summary>
    Vector2 GetPosition(VertexKey key);

    /// <summary>
    /// 指定された頂点群のバウンディングボックスを算出する
    /// </summary>
    BoundingBox ComputeBounds(IEnumerable<VertexKey> vertexKeys);

    /// <summary>
    /// 描画座標を論理座標に変換する
    /// </summary>
    Vector2 ToLogicalPosition(Vector2 screenPosition);
}
