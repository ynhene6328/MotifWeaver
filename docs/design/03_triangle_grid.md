# TriangleGridTopology

## 1. 座標系（倍密度）

```text
baseX = 2 * c
baseY = r
isUp = (r + c) % 2 == 0
```

---

## 2. 頂点

### 上向き

```text
(baseX, baseY)
(baseX + 2, baseY)
(baseX + 1, baseY + 1)
```

### 下向き

```text
(baseX + 1, baseY + 1)
(baseX, baseY + 2)
(baseX + 2, baseY + 2)
```

---

## 3. 実装

```csharp
Face BuildFace(int r, int c)
{
    ...
}
```

---

## 4. 特徴

- 整数座標
- 完全共有
- 浮動小数点不要

---