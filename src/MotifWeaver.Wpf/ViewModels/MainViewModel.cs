using System.Numerics;
using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;
using MotifWeaver.Rendering;

namespace MotifWeaver.Wpf.ViewModels;

public sealed class MainViewModel
{
    private readonly IGridGeometry _geometry;
    private readonly ITopologyQuery _query;
    private readonly ColorPalette _palette;

    public Pattern Pattern { get; }

    public MainViewModel()
    {
        Pattern = new Pattern(new HexGridTopology(), 5, 4);

        _geometry = new HexGridGeometry(40.0f);
        _query = new RayCastingTopologyQuery();
        _palette = new ColorPalette();

        _palette.SetColor(1, new Color(255, 0, 0));
    }

    public void OnClick(Vector2 screenPosition)
    {
        Vector2 logicalPosition = _geometry.ToLogicalPosition(screenPosition);
        Face? face = _query.FindFace(Pattern.Faces, logicalPosition);
        
        if (face != null)
        {
            face.AttributeId = 1;
        }
    }

    public void Render(IRenderer renderer)
    {
        var renderService = new RenderService(renderer, _geometry, _palette);
        renderService.Render(Pattern);
    }
}
