using System.Collections.Generic;
using System.Windows;
using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;
using MotifWeaver.Rendering;
using MotifWeaver.Wpf.Renderer;

namespace MotifWeaver.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        HexGridTopology topology = new HexGridTopology();
        IReadOnlyList<Face> faces = topology.Build(3, 3);
        
        HexGridGeometry geometry = new HexGridGeometry(40.0f);
        
        WpfRenderer renderer = new WpfRenderer(MainCanvas);
        RenderService renderService = new RenderService(renderer, geometry);
        
        renderService.Render(faces);
    }
}
