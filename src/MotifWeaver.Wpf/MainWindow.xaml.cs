using System.Windows;
using MotifWeaver.Wpf.Renderer;
using MotifWeaver.Wpf.ViewModels;

namespace MotifWeaver.Wpf;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var renderer = new WpfRenderer(MainCanvas);
        _viewModel.Render(renderer);
    }
}
