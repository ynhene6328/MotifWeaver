# WPF Renderer 設計（WpfRenderer）

## 概要

本ドキュメントは、WPFにおける描画処理を担当する `WpfRenderer` の設計を定義する。

`WpfRenderer` は `IRenderer` の実装であり、Core/Rendering層から渡される描画命令を、WPFの `Canvas` 上に描画する責務を持つ。

---

## 設計方針

### 基本方針

- 即時描画（Immediate Mode Rendering）を採用する
- 毎回全描画を再実行する
- 差分更新は行わない
- 状態を保持しない

---

### 設計原則

```
・Rendererは描画のみを担当する
・座標計算はGeometryに任せる
・トポロジー構造は一切知らない
・UIロジックを持たない
```

---

## クラス定義

```csharp
public sealed class WpfRenderer : IRenderer
{
    private readonly Canvas _canvas;

    public WpfRenderer(Canvas canvas);

    public void Begin();
    public void DrawPolygon(IReadOnlyList<Vector2> points, Color color);
    public void End();
}
```

---

## メソッド設計

### Begin()

```csharp
public void Begin()
{
    _canvas.Children.Clear();
}
```

#### 役割

- 描画前の初期化
- Canvas上の要素をすべて削除
- 再描画の前提を作る

---

### DrawPolygon()

#### 入力

```csharp
IReadOnlyList<Vector2> points
Color color
```

---

#### 処理概要

```
1. Polygonインスタンス生成
2. Vector2 → Point 変換
3. Pointsへ追加
4. Fill設定
5. Stroke設定
6. Canvasへ追加
```

---

#### 実装例

```csharp
public void DrawPolygon(IReadOnlyList<Vector2> points, Color color)
{
    if (points == null || points.Count < 3)
        return;

    var polygon = new Polygon();

    foreach (var p in points)
    {
        polygon.Points.Add(new Point(p.X, p.Y));
    }

    polygon.Fill = new SolidColorBrush(ToMediaColor(color));
    polygon.Stroke = Brushes.Black;
    polygon.StrokeThickness = 1.0;

    _canvas.Children.Add(polygon);
}
```

---

### End()

```csharp
public void End()
{
    // no-op
}
```

#### 役割

- 現時点では何も行わない
- 将来のバッチ処理や最適化の拡張ポイント

---

## カラー変換

Rendering層の `Color` をWPFの `Color` に変換する

```csharp
private static System.Windows.Media.Color ToMediaColor(Color c)
{
    return System.Windows.Media.Color.FromRgb(c.R, c.G, c.B);
}
```

---

## 座標系

### 前提

- 座標変換はすべて Geometry 層で完結している
- Rendererは変換済みの `Vector2` を受け取る

---

### Rendererの責務外

```
・スケーリング
・オフセット調整
・座標補正
```

---

## 描画対象

- 各 Face を1つの Polygon として描画する
- 頂点順序は Face.Vertices の順序をそのまま使用する

---

## 使用するWPF要素

- `Canvas`
- `Polygon`
- `Point`
- `SolidColorBrush`

---

## 描画フロー

```
Begin()
  ↓
DrawPolygon()（Faceごとに呼ばれる）
  ↓
End()
```

---

## パフォーマンス設計

### 現段階

- Canvas + Polygon を使用
- シンプルな実装を優先

---

### 将来の拡張

```
・DrawingVisualベースRendererへの差し替え
・差分描画の導入
```

---

## スレッド制約

- WPFのUI要素はUIスレッドでのみ操作可能
- RenderServiceはUIスレッドから呼び出される前提とする

---

## エラーハンドリング

最小限のチェックを行う

```
・頂点数 < 3 の場合は描画しない
```

---

## 拡張ポイント

### 線のスタイル

```
polygon.Stroke
polygon.StrokeThickness
```

---

### ハイライト

```
選択状態に応じたスタイル変更
```

---

### ZIndex制御

```
Canvas.SetZIndex()
```

---

## 禁止事項

```
・Polygonのキャッシュ保持
・差分更新ロジックの実装
・Topologyへの依存
・Geometry処理の実装
・ViewModelへの依存
```

---

## 使用例

```csharp
var topology = new HexGridTopology(...);
var geometry = new HexGridGeometry(...);

var renderer = new WpfRenderer(canvas);
var renderService = new RenderService(renderer, geometry);

renderService.Render(topology.Faces);
```

---

## 設計の要点

> Rendererは「描くだけ」の純粋な実装とする

> 再描画前提でシンプルに構築する

> Vector2の配列をPolygonとしてCanvasに配置するだけ

---

## まとめ

`WpfRenderer` は以下の特徴を持つ：

- 状態を持たない
- 毎回再描画する
- 描画に特化した単機能クラス
- Core/Geometry/Renderingとの責務分離を厳密に守る

これにより、将来的なBlazor対応や描画エンジンの差し替えが容易になる
