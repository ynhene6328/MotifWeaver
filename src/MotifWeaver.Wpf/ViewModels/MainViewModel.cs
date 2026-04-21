using System.Numerics;
using System.Linq;
using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;
using MotifWeaver.Rendering;
using MotifWeaver.Rendering.UseCases;

namespace MotifWeaver.Wpf.ViewModels;

public sealed class MainViewModel
{
    private readonly IGridGeometry _geometry;
    private readonly ITopologyQuery _query;
    private EditorRenderer _editorRenderer = null!;
    private ViewerRenderer _viewerRenderer = null!;
    private readonly IRenderer _editorCanvasRenderer;
    private readonly IRenderer _viewerCanvasRenderer;

    public Pattern Pattern { get; private set; } = null!;
    public PaletteViewModel Palette { get; }

    public MainViewModel(IRenderer editorRenderer, IRenderer viewerRenderer)
    {
        _editorCanvasRenderer = editorRenderer;
        _viewerCanvasRenderer = viewerRenderer;

        _geometry = new HexGridGeometry(40.0f);
        _query = new RayCastingTopologyQuery();
        
        Palette = new PaletteViewModel();

        CreatePattern(new PatternCreationParameters { GridType = GridType.Hex, Rows = 4, Cols = 4 });
    }

    public void CreatePattern(PatternCreationParameters p)
    {
        IGridTopology topology = p.GridType == GridType.Triangle 
            ? new TriangleGridTopology() 
            : new HexGridTopology();

        Pattern = new Pattern(topology, p.Rows, p.Cols);

        Func<int, Rendering.Color> colorResolver = id =>
        {
            var item = Palette.Items.FirstOrDefault(x => x.AttributeId == id);
            if (item != null)
                return ToRenderingColor(item.Color);
            return new Rendering.Color(200, 200, 200); // default
        };

        _editorRenderer = new EditorRenderer(_editorCanvasRenderer, _geometry, colorResolver);
        _viewerRenderer = new ViewerRenderer(_viewerCanvasRenderer, _geometry, colorResolver);
    }

    private static Rendering.Color ToRenderingColor(System.Windows.Media.Color color)
    {
        return new Rendering.Color(color.R, color.G, color.B);
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
