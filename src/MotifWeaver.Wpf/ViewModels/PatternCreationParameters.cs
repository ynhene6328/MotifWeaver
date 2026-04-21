namespace MotifWeaver.Wpf.ViewModels;

public enum GridType
{
    Triangle,
    Hex
}

public sealed class PatternCreationParameters
{
    public GridType GridType { get; set; }
    public int Rows { get; set; }
    public int Cols { get; set; }
}
