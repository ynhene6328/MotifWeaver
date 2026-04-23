using System;
using System.Numerics;
using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Rendering.UseCases;

public sealed class ViewerRenderer
{
    private readonly IRenderer _renderer;
    private readonly RenderService _renderService;

    public ViewerRenderer(IRenderer renderer, IGridGeometry geometry, Func<int, Color> colorResolver)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _renderService = new RenderService(renderer, geometry, colorResolver);
    }

    public void Render(Pattern pattern, float width, float height)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        var logicalWidth = pattern.Topology.CalculateLogicalWidth();
        var logicalHeight = pattern.Topology.CalculateLogicalHeight();

        var logicalSize = _renderService.Geometry.ToLogicalPosition(new Vector2(width, height));
        var repeatX = (int)(logicalSize.X / logicalWidth) + 1;
        var repeatY = (int)(logicalSize.Y / logicalHeight) + 1;

        _renderer.Begin();

        for (int y = 0; y < repeatY; y++)
        {
            for (int x = 0; x < repeatX; x++)
            {
                var offset = new Vector2(x * logicalWidth, y * logicalHeight);
                _renderService.Render(pattern, offset);
            }
        }

        _renderer.End();
    }
}
