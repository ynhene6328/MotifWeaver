// /src/MotifWeaver.Rendering/IRenderer.cs
using System.Collections.Generic;
using System.Numerics;

namespace MotifWeaver.Rendering;

/// <summary>
/// 描画命令の受け口を定義するインターフェース
/// Topology・Geometryには一切依存しない
/// </summary>
public interface IRenderer
{
    /// <summary>
    /// 描画フレームを開始する
    /// </summary>
    void Begin();

    /// <summary>
    /// 多角形を描画する
    /// </summary>
    void DrawPolygon(IReadOnlyList<Vector2> points, Color fillColor);

    /// <summary>
    /// 描画フレームを確定する
    /// </summary>
    void End();
}
