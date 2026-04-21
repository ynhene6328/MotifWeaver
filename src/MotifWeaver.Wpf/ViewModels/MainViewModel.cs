using System;
using System.Collections.Specialized;
using System.ComponentModel;
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
    private float _lastViewerWidth;
    private float _lastViewerHeight;

    public Pattern Pattern { get; private set; } = null!;
    public PaletteViewModel Palette { get; }

    public MainViewModel(IRenderer editorRenderer, IRenderer viewerRenderer)
    {
        _editorCanvasRenderer = editorRenderer;
        _viewerCanvasRenderer = viewerRenderer;

        _geometry = new HexGridGeometry(40.0f);
        _query = new RayCastingTopologyQuery();
        
        Palette = new PaletteViewModel();

        // Palette.Items の変更イベントをリッスン
        Palette.Items.CollectionChanged += PaletteItems_CollectionChanged;

        // 既存アイテムのPropertyChangedイベントをリッスン
        foreach (var item in Palette.Items)
        {
            AttachItemPropertyChanged(item);
        }

        CreatePattern(new PatternCreationParameters { GridType = GridType.Hex, Rows = 4, Cols = 4 });
    }

    private void PaletteItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // 追加されたアイテムのPropertyChangedをリッスン
        if (e.NewItems != null)
        {
            foreach (PaletteItemViewModel item in e.NewItems)
            {
                AttachItemPropertyChanged(item);
            }
        }

        // 削除されたアイテムのPropertyChangedをリッスン解除
        if (e.OldItems != null)
        {
            foreach (PaletteItemViewModel item in e.OldItems)
            {
                DetachItemPropertyChanged(item);
            }
        }
    }

    private void AttachItemPropertyChanged(PaletteItemViewModel item)
    {
        item.PropertyChanged += PaletteItem_PropertyChanged;
    }

    private void DetachItemPropertyChanged(PaletteItemViewModel item)
    {
        item.PropertyChanged -= PaletteItem_PropertyChanged;
    }

    private void PaletteItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Colorプロパティが変更されたときにRenderをトリガー
        if (e.PropertyName == nameof(PaletteItemViewModel.Color))
        {
            Render(_lastViewerWidth, _lastViewerHeight);
        }
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
        
        if (face != null && Palette.SelectedItem != null)
        {
            face.AttributeId = Palette.SelectedItem.AttributeId;
        }
    }

    public void Render(float viewerWidth, float viewerHeight)
    {
        _lastViewerWidth = viewerWidth;
        _lastViewerHeight = viewerHeight;
        
        _editorRenderer.Render(Pattern);
        _viewerRenderer.Render(Pattern, viewerWidth, viewerHeight);
    }
}
