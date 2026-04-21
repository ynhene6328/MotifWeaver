// /src/MotifWeaver.Rendering/RenderService.cs
using System;
using System.Collections.Generic;
using System.Numerics;
using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Rendering;

/// <summary>
/// Face → Geometry(Vector2変換) → IRenderer(描画) の橋渡しを行う
/// </summary>
public sealed class RenderService
{
    private readonly IRenderer _renderer;
    private readonly IGridGeometry _geometry;
    public IGridGeometry Geometry => _geometry;
    private readonly Func<int, Color> _colorResolver;

    public RenderService(IRenderer renderer, IGridGeometry geometry, Func<int, Color> colorResolver)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
        _colorResolver = colorResolver ?? throw new ArgumentNullException(nameof(colorResolver));
    }



    public void Render(Pattern pattern, Vector2 logicalOffset)
    {
        if (pattern is null)
        {
            throw new ArgumentNullException(nameof(pattern));
        }

        foreach (Face face in pattern.Faces)
        {
            List<Vector2> points = new List<Vector2>(face.Vertices.Count);

            for (int index = 0; index < face.Vertices.Count; index++)
            {
                VertexKey key = face.Vertices[index].Key;
                var shifted = new VertexKey(
                    key.X + (int)logicalOffset.X,
                    key.Y + (int)logicalOffset.Y);

                Vector2 position = _geometry.GetPosition(shifted);
                points.Add(position);
            }

            Color fillColor = _colorResolver(face.AttributeId);
            _renderer.DrawPolygon(points, fillColor);
        }
    }
}
