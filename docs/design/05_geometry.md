# Geometry設計（強化版）

## 1. 目的

Topologyの整数座標を描画座標へ変換する。

---

## 2. 変換

### Triangle

```text
x = (X / 2) * w
y = Y * (√3 / 2) * w
```

### Hex

```text
x = X * (w / 2)
y = Y * (√3 / 2) * w
```

---

## 3. BoundingBox

```csharp
BoundingBox ComputeBounds(IEnumerable<Vertex> vertices)
{
    var points = vertices.Select(v => GetPosition(v));

    return new BoundingBox
    {
        MinX = points.Min(p => p.X),
        MaxX = points.Max(p => p.X),
        MinY = points.Min(p => p.Y),
        MaxY = points.Max(p => p.Y)
    };
}
```

---

## 4. タイルサイズ

```csharp
tileWidth  = MaxX - MinX;
tileHeight = MaxY - MinY;
```

---

## 5. キャッシュ

```csharp
Dictionary<Vertex, Vector2> positionCache;
```

---

## 6. 設計原則

- 描画専用
- 浮動小数点許可
- 再計算最小化

---