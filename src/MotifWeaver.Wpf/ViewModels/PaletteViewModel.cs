using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MotifWeaver.Wpf.Mvvm;

namespace MotifWeaver.Wpf.ViewModels;

public sealed class PaletteViewModel : ViewModelBase
{
    private PaletteItemViewModel? _selectedItem;

    public ObservableCollection<PaletteItemViewModel> Items { get; }

    public PaletteItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    public ICommand AddCommand { get; }
    public ICommand ChangeColorCommand { get; }

    public PaletteViewModel()
    {
        Items = new ObservableCollection<PaletteItemViewModel>();

        AddCommand = new DelegateCommand(_ =>
        {
            int nextId = Items.Count > 0 ? Items.Max(x => x.AttributeId) + 1 : 1;
            Random rand = new Random();
            System.Windows.Media.Color newColor = System.Windows.Media.Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256));
            Items.Add(new PaletteItemViewModel(nextId, newColor));
        });

        ChangeColorCommand = new DelegateCommand(param =>
        {
            if (param is PaletteItemViewModel item)
            {
                using var dialog = new System.Windows.Forms.ColorDialog();
                dialog.Color = System.Drawing.Color.FromArgb(255, item.Color.R, item.Color.G, item.Color.B);
                
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    item.Color = System.Windows.Media.Color.FromRgb(dialog.Color.R, dialog.Color.G, dialog.Color.B);
                }
            }
        });

        // Default item configurations
        Items.Add(new PaletteItemViewModel(0, System.Windows.Media.Colors.White));
        Items.Add(new PaletteItemViewModel(1, System.Windows.Media.Colors.Red));
        
        // 初期選択
        if (Items.Count > 0)
        {
            SelectedItem = Items[0];
        }
    }
}
