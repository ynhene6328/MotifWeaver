using System;
using System.Windows;
using MotifWeaver.Wpf.ViewModels;

namespace MotifWeaver.Wpf.Views;

public partial class NewPatternDialog : Window
{
    public PatternCreationParameters? Parameters { get; private set; }

    public NewPatternDialog()
    {
        InitializeComponent();
        GridTypeComboBox.ItemsSource = Enum.GetValues(typeof(GridType));
        GridTypeComboBox.SelectedIndex = 1; // Default Hex
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(RowsTextBox.Text, out int rows) && int.TryParse(ColsTextBox.Text, out int cols))
        {
            if (cols % 2 != 0)
            {
                System.Windows.MessageBox.Show("Cols must be an even number.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            Parameters = new PatternCreationParameters
            {
                GridType = (GridType)GridTypeComboBox.SelectedItem,
                Rows = rows,
                Cols = cols
            };
            DialogResult = true;
            Close();
        }
        else
        {
            System.Windows.MessageBox.Show("Invalid input for Rows or Cols.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }
}
