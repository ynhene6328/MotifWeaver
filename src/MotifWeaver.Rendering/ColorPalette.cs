using System.Collections.Generic;

namespace MotifWeaver.Rendering;

public sealed class ColorPalette
{
    private readonly Dictionary<int, Color> _colors;

    public ColorPalette()
    {
        _colors = new Dictionary<int, Color>();
    }

    public void SetColor(int attributeId, Color color)
    {
        _colors[attributeId] = color;
    }

    public Color GetColor(int attributeId)
    {
        if (_colors.TryGetValue(attributeId, out var color))
        {
            return color;
        }

        return new Color(255, 255, 255); // デフォルト白
    }
}
