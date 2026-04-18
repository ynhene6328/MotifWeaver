# MotifWeaver UI設計書（WPF / Blazor対応方針）

## 概要

本ドキュメントは、MotifWeaverにおけるUI層（WPF / Blazor）の設計方針およびアーキテクチャ構成を定義する。

本プロジェクトは以下の思想に基づく：

- コアロジック（Topology / Geometry / Rendering）は完全にUI非依存
- UIは差し替え可能なレイヤとして構築する
- WPFとBlazorの両対応を前提とするが、UIは共通化しない
- 共通化するのは「状態」と「操作ロジック」に限定する

---

## 全体アーキテクチャ

```
MotifWeaver.Core
  ├─ Topology
  └─ Geometry

MotifWeaver.Rendering
  ├─ IRenderer
  └─ RenderService

MotifWeaver.Application（新規）
  ├─ 状態管理
  └─ 操作ロジック

MotifWeaver.Wpf
  ├─ View（XAML）
  ├─ ViewModel（ReactiveProperty）
  └─ WpfRenderer

MotifWeaver.Web（将来）
  ├─ Component（Blazor）
  └─ CanvasRenderer
```

---

## レイヤ責務

### Core（既存）

- トポロジー構造（Vertex / Edge / Face）
- 幾何変換（整数座標 → Vector2）
- 完全にUI非依存

---

### Rendering（既存）

- 描画抽象（IRenderer）
- RenderServiceによる橋渡し

責務：

```
Face → Geometry → Vector2 → Renderer
```

---

### Application（新規）

UI非依存の「状態」と「操作」を管理する層

#### 目的

- WPF / Blazor間でロジックを共有
- UIの違いを吸収

#### 含むもの

- グリッド定義（サイズなど）
- Faceごとの色情報
- 色変更などの操作ロジック

#### 含まないもの

- UIフレームワーク依存コード
- ReactiveProperty
- 描画処理

---

### WPF

#### 採用パターン

- MVVM
- ReactiveProperty

#### 構成

```
Views/
ViewModels/
Renderer/
```

---

### Blazor（将来）

#### 構成

```
Components/
Renderer/
```

#### 特徴

- ViewModelは持たない
- Component Stateで管理

---

## 共通化戦略

### 共通化するもの

```
・データ構造（FaceColorなど）
・操作ロジック（色変更など）
・グリッド定義
```

---

### 共通化しないもの

```
・View（XAML / HTML）
・ViewModel
・イベント処理
・描画実装
```

---

## ReactivePropertyの扱い

### 方針

- WPFのみで使用する
- Application層には持ち込まない

### 理由

- WPF依存（INotifyPropertyChangedベース）
- Blazorとは相性が悪い

---

## WPF設計

### ViewModel

例：

```
MainViewModel
  ├─ GridWidth
  ├─ GridHeight
  ├─ SelectedColor
  ├─ FaceColors
  └─ Commands
```

---

### Renderer

```
WpfRenderer : IRenderer
```

責務：

- Vector2をCanvasやDrawingContextへ描画
- 座標計算は行わない

---

### 描画フロー

```
Topology → Geometry → RenderService → WpfRenderer → Canvas
```

---

## Blazor設計（将来）

### 構成

- Componentが状態を保持
- RendererがCanvas描画を担当

---

### 描画フロー

```
Topology → Geometry → RenderService → CanvasRenderer → HTML Canvas
```

---

## 設計原則

### 1. 責務分離

```
Topology = 構造
Geometry = 座標変換
Rendering = 描画命令
UI = 表示と操作
```

---

### 2. 依存方向

```
UI → Application → Core
             ↓
        Rendering
```

---

### 3. Rendererの制約

- Topologyを知らない
- Geometryを知らない
- Vector2のみ扱う

---

### 4. RenderServiceの制約

- ロジックを持たない
- 変換と中継のみ

---

## 最初に実装するUI（WPF）

### Viewer（最小構成）

- HexGridを描画
- 色は固定（グレー）
- RenderServiceを使用

---

### 非対象（初期段階）

- 編集機能（Editor）
- 複雑なUI操作
- 入力制御

---

## 今後の拡張

### 1. Editor機能

- Faceクリックで色変更
- 選択状態管理

---

### 2. Edgeカラー

- Edge単位の色管理
- 描画ロジック拡張

---

### 3. パス探索

- 同色領域の一筆書き
- グラフ探索アルゴリズム

---

### 4. 描画拡張

- ズーム
- パン
- ハイライト

---

## まとめ

本設計は以下を実現する：

- コアロジックの完全分離
- UI差し替え可能な構造
- WPF / Blazor両対応
- 高いテスト容易性

---

## 設計の要点（最重要）

> 「状態は共通、UIは別物」

> 「Geometryが座標、Rendererは描くだけ」

> 「RenderServiceは翻訳者」
