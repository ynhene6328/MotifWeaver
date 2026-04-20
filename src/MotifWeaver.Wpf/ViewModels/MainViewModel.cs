using System.Numerics;
using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;
using MotifWeaver.Rendering;
using MotifWeaver.Rendering.UseCases;

namespace MotifWeaver.Wpf.ViewModels;

public sealed class MainViewModel
{
    private readonly IGridGeometry _geometry;
    private readonly ITopologyQuery _query;
    private readonly ColorPalette _palette;

    private readonly EditorRenderer _editorRenderer;
    private readonly ViewerRenderer _viewerRenderer;

    public Pattern Pattern { get; }

    public MainViewModel(IRenderer editorRenderer, IRenderer viewerRenderer)
    {
        Pattern = new Pattern(new HexGridTopology(), 4, 4);

        _geometry = new HexGridGeometry(40.0f);
        _query = new RayCastingTopologyQuery();
        _palette = new ColorPalette();

        _palette.SetColor(1, new Color(255, 0, 0));

        _editorRenderer = new EditorRenderer(editorRenderer, _geometry, _palette);
        _viewerRenderer = new ViewerRenderer(viewerRenderer, _geometry, _palette);
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

    public void Render(float viewerWidth, float viewerHeight)
    {
        _editorRenderer.Render(Pattern);
        _viewerRenderer.Render(Pattern, viewerWidth, viewerHeight);
    }
}
