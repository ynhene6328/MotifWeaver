// /src/MotifWeaver.Rendering/Color.cs

namespace MotifWeaver.Rendering;

/// <summary>
/// 描画用の色を表す不変構造体（外部ライブラリ非依存）
/// </summary>
public readonly struct Color
{
    public Color(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public byte R { get; }

    public byte G { get; }

    public byte B { get; }
}
