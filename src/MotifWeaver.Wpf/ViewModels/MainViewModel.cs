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
        Pattern = new Pattern(new HexGridTopology(), 4, 4);

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
            face.AttributeId = face.AttributeId == 0 ? 1 : 0;
        }
    }

    public void Render(IRenderer editorRenderer, IRenderer viewerRenderer, float viewerWidth, float viewerHeight)
    {
        var editorService = new RenderService(editorRenderer, _geometry, _palette);
        editorService.Render(Pattern);

        var viewerService = new RenderService(viewerRenderer, _geometry, _palette);
        var repeatService = new ViewerRenderService(viewerService);
        
        viewerRenderer.Begin(); // 描画前にクリア
        repeatService.Render(Pattern, viewerWidth, viewerHeight);
        viewerRenderer.End();
    }
}
