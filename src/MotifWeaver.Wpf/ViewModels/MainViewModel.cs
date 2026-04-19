using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;
using MotifWeaver.Rendering;

namespace MotifWeaver.Wpf.ViewModels;

public sealed class MainViewModel
{
    public void Render(IRenderer renderer)
    {
        var topology = new HexGridTopology();
        var faces = topology.Build(5, 5);

        var geometry = new HexGridGeometry(40.0f);

        var palette = new ColorPalette();
        palette.SetColor(1, new Color(255, 0, 0));

        // 確認のため最初のFaceにAttributeId=1を設定
        if (faces.Count > 0)
        {
            faces[0].AttributeId = 1;
        }

        var renderService = new RenderService(renderer, geometry, palette);
        renderService.Render(faces);
    }
}
