using System.Numerics;
using System.Windows;
using System.Windows.Input;
using MotifWeaver.Wpf.Renderer;
using MotifWeaver.Wpf.ViewModels;

namespace MotifWeaver.Wpf;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private WpfRenderer? _editorRenderer;
    private WpfRenderer? _viewerRenderer;

    public MainWindow()
    {
        InitializeComponent();
        _editorRenderer = new WpfRenderer(EditorCanvas);
        _viewerRenderer = new WpfRenderer(ViewerCanvas);
        _viewModel = new MainViewModel(_editorRenderer, _viewerRenderer);
        DataContext = _viewModel;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _viewModel.Render((float)ViewerCanvas.ActualWidth, (float)ViewerCanvas.ActualHeight);
    }

    private void MainCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        System.Windows.Point position = e.GetPosition(EditorCanvas);
        _viewModel.OnClick(new Vector2((float)position.X, (float)position.Y));
        _viewModel.Render((float)ViewerCanvas.ActualWidth, (float)ViewerCanvas.ActualHeight);
    }

    private void ViewerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _viewModel.Render((float)ViewerCanvas.ActualWidth, (float)ViewerCanvas.ActualHeight);
    }
}
