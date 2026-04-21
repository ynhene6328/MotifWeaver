using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MotifWeaver.Wpf;

/// <summary>
/// System.Windows.Media.Color を SolidColorBrush に変換するコンバーター
/// </summary>
public sealed class ColorToSolidColorBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is System.Windows.Media.Color color)
        {
            return new SolidColorBrush(color);
        }

        return System.Windows.Data.Binding.DoNothing;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            return brush.Color;
        }

        return System.Windows.Data.Binding.DoNothing;
    }
}
