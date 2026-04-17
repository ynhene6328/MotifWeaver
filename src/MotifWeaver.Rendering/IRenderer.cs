// /src/MotifWeaver.Rendering/IRenderer.cs
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace MotifWeaver.Rendering;

public interface IRenderer
{
    void Begin();

    void DrawPolygon(IReadOnlyList<Vector2> points, Color color);

    void End();
}
