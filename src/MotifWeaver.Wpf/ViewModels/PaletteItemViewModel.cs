using MotifWeaver.Wpf.Mvvm;

namespace MotifWeaver.Wpf.ViewModels;

public sealed class PaletteItemViewModel : ViewModelBase
{
    private System.Windows.Media.Color _color;

    public int AttributeId { get; }

    public System.Windows.Media.Color Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    public PaletteItemViewModel(int attributeId, System.Windows.Media.Color color)
    {
        AttributeId = attributeId;
        _color = color;
    }
}
