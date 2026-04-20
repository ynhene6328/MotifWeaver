using System;
using System.Numerics;
using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Rendering.UseCases;

public sealed class EditorRenderer
{
    private readonly IRenderer _renderer;
    private readonly RenderService _renderService;

    public EditorRenderer(IRenderer renderer, IGridGeometry geometry, ColorPalette palette)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _renderService = new RenderService(renderer, geometry, palette);
    }

    public void Render(Pattern pattern)
    {
        if (pattern == null)
            throw new ArgumentNullException(nameof(pattern));

        _renderer.Begin();
        _renderService.Render(pattern, Vector2.Zero);
        _renderer.End();
    }
}
