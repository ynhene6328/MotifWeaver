# Pattern / Tiling 設計

## 概要

本アプリケーションにおける主要機能は、  
Editorで作成したパターンをViewer上で繰り返し描画することである。

そのため、単一のTopology（Face集合）を描画するだけでは不十分であり、  
「パターン単位」と「繰り返し配置」の概念を導入する必要がある。

---

## 設計方針

- Topologyは純粋な構造として維持（変更しない）
- Geometryは座標変換のみ担当（変更しない）
- Renderingは描画のみ担当（変更しない）
- 繰り返しロジックは新たなレイヤとして追加する

---

## 概念整理

本システムは以下の3階層で構成される。

### 1. Face（最小単位）

- 単一のポリゴン（六角形・三角形など）
- Topologyで定義される

---

### 2. Pattern（編集単位）

- 複数のFaceをまとめたもの
- 各Faceに色を持たせる
- Editorで操作される単位

例：
- 4×4のモチーフ集合

---

### 3. Tiling（描画単位）

- Patternを平面に繰り返し配置したもの
- Viewerで表示される単位

例：
- Patternを8×8で繰り返す

---

## クラス設計

### Pattern

```csharp
public sealed class Pattern
{
    public IReadOnlyList<Face> Faces { get; }

    // Faceごとの色
    public IReadOnlyDictionary<Face, Color> FaceColors { get; }

    // Patternのサイズ（タイル単位）
    public int Width { get; }
    public int Height { get; }
}
```

#### 責務

- Face集合の保持
- Faceごとの色情報の管理
- パターンサイズの定義

---

### PatternInstance

```csharp
public readonly struct PatternInstance
{
    public Pattern Pattern { get; }
    public int OffsetX { get; }
    public int OffsetY { get; }
}
```

#### 責務

- Patternの配置位置を表現
- 平面上の繰り返し位置を定義

---

### TilingGenerator

```csharp
public sealed class TilingGenerator
{
    public IEnumerable<PatternInstance> Generate(
        Pattern pattern,
        int repeatX,
        int repeatY)
    {
        for (int y = 0; y < repeatY; y++)
        {
            for (int x = 0; x < repeatX; x++)
            {
                yield return new PatternInstance(pattern, x, y);
            }
        }
    }
}
```

#### 責務

- Patternを指定回数繰り返して配置する
- PatternInstance列を生成する

---

## 描画フロー

### 現状

```text
Face → Geometry → Renderer
```

---

### 拡張後

```text
PatternInstance
 → Pattern
   → Face
     → VertexKey + Offset
       → Geometry
         → Renderer
```

---

## オフセット処理

### 方針

- オフセットはVertexKeyに対して適用する
- Geometryは変更しない

---

### 実装イメージ

```csharp
foreach (var instance in instances)
{
    foreach (var face in instance.Pattern.Faces)
    {
        var transformedVertices = face.Vertices
            .Select(v => new VertexKey(
                v.X + instance.OffsetX * patternWidth,
                v.Y + instance.OffsetY * patternHeight));

        // GeometryでVector2へ変換し描画
    }
}
```

---

## 重要設計ポイント

### 1. Pattern単位でオフセットを適用する

#### 正しい設計

```text
Pattern全体を同じオフセットで移動
```

#### 誤った設計

```text
Face単位で個別にオフセット
```

→ パターンが崩壊する

---

### 2. Topologyを再生成しない

```text
Patternは1回構築し再利用する
```

---

### 3. Geometryは不変

```text
VertexKey → Vector2 の変換のみ
```

---

## パターンサイズの扱い

Patternの繰り返しにはサイズ情報が必要となる。

```text
offsetX * patternWidth
offsetY * patternHeight
```

この値をVertexKeyに加算することで平行移動を実現する。

---

## 拡張性

本設計は以下の拡張に対応可能である。

### 回転

- VertexKeyの変換で対応可能

---

### 反転

- X / Y座標の反転で対応可能

---

### スケール

- Geometry側で対応可能

---

## レイヤ構成

```text
Topology（構造）
Geometry（座標変換）
Pattern（意味・色）
Tiling（配置）
Rendering（描画）
```

---

## まとめ

- Patternを編集単位とすることで、EditorとViewerを分離できる
- Tilingを導入することで繰り返し描画が可能になる
- 既存のTopology / Geometry / Renderingを変更せず拡張できる
- オフセットはVertexKeyに対して適用することでシンプルに実現できる

本設計により、アプリケーションの主要機能である  
「パターンの繰り返し描画」を安全かつ拡張可能な形で実現できる。