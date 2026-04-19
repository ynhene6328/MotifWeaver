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

        var renderService = new RenderService(renderer, geometry);
        renderService.Render(faces);
    }
}
