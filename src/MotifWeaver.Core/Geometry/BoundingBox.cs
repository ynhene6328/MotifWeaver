// /src/MotifWeaver.Core/Geometry/BoundingBox.cs

namespace MotifWeaver.Core.Geometry;

/// <summary>
/// 描画領域の境界を表す不変構造体
/// </summary>
public readonly struct BoundingBox
{
    public BoundingBox(float minX, float minY, float maxX, float maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    public float MinX { get; }

    public float MinY { get; }

    public float MaxX { get; }

    public float MaxY { get; }

    /// <summary>
    /// タイル幅（= MaxX - MinX）
    /// </summary>
    public float Width => MaxX - MinX;

    /// <summary>
    /// タイル高さ（= MaxY - MinY）
    /// </summary>
    public float Height => MaxY - MinY;
}
