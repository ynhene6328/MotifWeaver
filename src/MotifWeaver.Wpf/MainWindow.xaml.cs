using System.Numerics;
using System.Windows;
using System.Windows.Input;
using MotifWeaver.Wpf.Renderer;
using MotifWeaver.Wpf.ViewModels;

namespace MotifWeaver.Wpf;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private WpfRenderer? _renderer;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _renderer = new WpfRenderer(MainCanvas);
        _viewModel.Render(_renderer);
    }

    private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_renderer == null)
            return;

        Point position = e.GetPosition(MainCanvas);
        _viewModel.OnClick(new Vector2((float)position.X, (float)position.Y));
        _viewModel.Render(_renderer);
    }
}
