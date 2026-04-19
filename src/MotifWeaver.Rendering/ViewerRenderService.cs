using System;
using System.Numerics;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Rendering;

public sealed class ViewerRenderService
{
    private readonly RenderService _renderService;

    public ViewerRenderService(RenderService renderService)
    {
        _renderService = renderService ?? throw new ArgumentNullException(nameof(renderService));
    }

    public void Render(Pattern pattern, float width, float height)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        // Topologyから論理サイズを取得
        var (dy, dx) = pattern.Topology.CalculateSize(pattern.Faces);

        if (dx == 0) dx = 6; // HexGridの場合の暫定論理幅 (例: cxが幅なので6にする)
        if (dy == 0) dy = 6; // HexGridの場合の暫定論理高さ

        // 暫定：固定回数でも可（後で改善）
        int repeatX = 10;
        int repeatY = 10;

        for (int y = 0; y < repeatY; y++)
        {
            for (int x = 0; x < repeatX; x++)
            {
                var offset = new Vector2(x * dx, y * dy);
                _renderService.Render(pattern, offset);
            }
        }
    }
}
