using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MotifWeaver.Rendering;
using Color = MotifWeaver.Rendering.Color;

namespace MotifWeaver.Wpf.Renderer;

public sealed class WpfRenderer : IRenderer
{
    private readonly Canvas _canvas;

    public WpfRenderer(Canvas canvas)
    {
        _canvas = canvas;
    }

    public void Begin()
    {
        _canvas.Children.Clear();
    }

    public void DrawPolygon(IReadOnlyList<Vector2> points, Color fillColor)
    {
        Polygon polygon = new Polygon
        {
            Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(fillColor.R, fillColor.G, fillColor.B)),
            Stroke = System.Windows.Media.Brushes.Gray,
            StrokeThickness = 0.1
        };

        for (int i = 0; i < points.Count; i++)
        {
            polygon.Points.Add(new System.Windows.Point(points[i].X, points[i].Y));
        }

        _canvas.Children.Add(polygon);
    }

    public void End()
    {
    }

    public void SetCanvasSize(int width, int height)
    {
        _canvas.Width = width;
        _canvas.Height = height;
    }
}
