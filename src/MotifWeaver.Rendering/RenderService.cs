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

    public RenderService(IRenderer renderer, IGridGeometry geometry)
    {
        _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        _geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
    }

    /// <summary>
    /// 指定されたFace群を描画する
    /// </summary>
    public void Render(IEnumerable<Face> faces)
    {
        if (faces is null)
        {
            throw new ArgumentNullException(nameof(faces));
        }

        _renderer.Begin();

        foreach (Face face in faces)
        {
            List<Vector2> points = new List<Vector2>(face.Vertices.Count);

            for (int index = 0; index < face.Vertices.Count; index++)
            {
                Vector2 position = _geometry.GetPosition(face.Vertices[index].Key);
                points.Add(position);
            }

            // System.Drawing.Color → Rendering.Color への変換
            Color fillColor = new Color(face.Color.R, face.Color.G, face.Color.B);
            _renderer.DrawPolygon(points, fillColor);
        }

        _renderer.End();
    }
}
