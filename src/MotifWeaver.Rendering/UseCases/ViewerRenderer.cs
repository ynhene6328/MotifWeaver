using System;
using System.Numerics;
using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Rendering.UseCases;

public sealed class ViewerRenderer
{
    private readonly IRenderer _renderer;
    private readonly RenderService _renderService;

    public ViewerRenderer(IRenderer renderer, IGridGeometry geometry, ColorPalette palette)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _renderService = new RenderService(renderer, geometry, palette);
    }

    public void Render(Pattern pattern, float width, float height)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        var (dy, dx) = pattern.Topology.CalculateSize(pattern.Faces);

        if (dx == 0) dx = 6;
        if (dy == 0) dy = 6;

        int repeatX = 10;
        int repeatY = 10;

        _renderer.Begin();

        for (int y = 0; y < repeatY; y++)
        {
            for (int x = 0; x < repeatX; x++)
            {
                var offset = new Vector2(x * dx, y * dy);
                _renderService.Render(pattern, offset);
            }
        }

        _renderer.End();
    }
}
