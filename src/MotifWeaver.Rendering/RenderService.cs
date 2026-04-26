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

        var edges = new HashSet<Edge>();
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

            foreach (var edge in face.Edges)
            {
                edges.Add(edge);
            }
        }

        foreach(var edge in edges)
        {
            var v1 = edge.V1.Key;
            var v2 = edge.V2.Key;

            var p1 = _geometry.GetPosition(new VertexKey(v1.X + (int)logicalOffset.X, v1.Y + (int)logicalOffset.Y));
            var p2 = _geometry.GetPosition(new VertexKey(v2.X + (int)logicalOffset.X, v2.Y + (int)logicalOffset.Y));

            Vector2 direction = Vector2.Normalize(p2 - p1);
            Vector2 normal = new Vector2(-direction.Y, direction.X);
            float thickness = 4.0f; // 線の太さ
            Vector2 offset = normal * thickness / 2;

            List<Vector2> edgePoints = new List<Vector2>
            {
                p1 + offset,
                p1 - offset,
                p2 - offset,
                p2 + offset
            };

            Color edgeColor = _colorResolver(edge.AttributeId);
            _renderer.DrawPolygon(edgePoints, edgeColor);            
        }
    }

    public void SetCanvasSize(int logicalWidth, int logicalHeight)
    {
        var temp = _geometry.GetPosition(new VertexKey(logicalWidth, logicalHeight));
        _renderer.SetCanvasSize((int)temp.X, (int)temp.Y);
    }
}
