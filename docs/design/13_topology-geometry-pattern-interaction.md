# 13. Topology / Geometry / Pattern / Interaction 設計

## 1. 目的

本ドキュメントでは以下を明確化する：

- Topology / Geometry / Pattern 各ドメインの責務分離
- EditorにおけるFace特定フロー
- PatternのResize処理の責務分離と実行フロー

---

## 2. ドメイン責務の整理

### 2.1 Topologyドメイン

#### 概要
Topologyは「構造」と「構造に対する幾何的操作」を扱う。

#### 責務

##### ① 構造定義
- Vertex / Edge / Face
- 接続関係（隣接）
- VertexKey（整数座標）

##### ② 生成（Generator）
- HexGridTopology
- TriangleGridTopology

```
IReadOnlyList<Face> Build(int rows, int cols)
```

##### ③ 問い合わせ（Query）
- Face特定（HitTest用）
- 幾何判定（面内判定など）

例：
```
Face? FindFace(IReadOnlyList<Face> faces, Vector2 logicalPosition)
```

##### ④ 変換（Transform）
- PatternのResize
- 属性引き継ぎ

例：
```
Pattern Resize(Pattern oldPattern, int rows, int cols)
```

#### 注意

- HexGridTopology / TriangleGridTopology は「生成専用」
- Query / Transform は別クラスとして分離する

---

### 2.2 Geometryドメイン

#### 概要
論理座標と描画座標の変換を担当する

#### 責務

- VertexKey → Vector2（描画座標）
- Vector2 → 論理座標（実数空間）

例：
```
Vector2 GetPosition(VertexKey key)
Vector2 ToLogical(Vector2 screenPosition)
```

#### 非責務

- Face特定
- トポロジー構造の解釈

---

### 2.3 Patternクラス

#### 概要
ユーザが編集する「Topologyインスタンス」を保持する

#### 保持内容

- IReadOnlyList<Face>
- Rows / Cols
- （必要に応じて）検索用インデックス

#### 責務

- Face集合の保持
- Resize操作の入口
- Query対象としての提供

#### 非責務

- 幾何計算
- 座標変換
- Face特定アルゴリズム

---

## 3. Face特定フロー（Editor）

### 3.1 概要

ユーザのクリックからFaceを特定するまでの流れ

---

### 3.2 フロー

```
[1] ユーザクリック（screen座標）
    ↓
[2] Geometry
    screen → logical（実数座標）
    ↓
[3] TopologyQuery
    logical → Face
    ↓
[4] Editor
    Faceに対する操作（色変更など）
```

---

### 3.3 ポイント

- Patternは探索ロジックを持たない
- Geometryは座標変換のみ
- Face特定はTopologyドメインに属する

---

## 4. Pattern Resize設計

### 4.1 要件

- 既存のPatternを維持しつつサイズ変更
- AttributeIDを可能な限り引き継ぐ

---

### 4.2 責務分離

#### Pattern

- Resizeの入口を提供

```
Pattern Resize(int rows, int cols, ITopologyResizer resizer)
```

---

#### TopologyTransform（Resizer）

- 新Topology生成
- Attribute引き継ぎ
- Pattern再構築

---

### 4.3 フロー

```
[1] Editor / ViewModel
    pattern.Resize(...)
    ↓
[2] TopologyResizer
    ↓
    (a) 新Face生成（Generator使用）
    (b) 旧Faceとの対応付け
    (c) AttributeID引き継ぎ
    ↓
[3] 新Pattern生成
    ↓
[4] 呼び出し元へ返却
```

---

### 4.4 実装イメージ

```
public interface ITopologyResizer
{
    Pattern Resize(Pattern oldPattern, int rows, int cols);
}
```

---

### 4.5 重要な分離

| 要素 | 役割 |
|------|------|
| Generator | 新規生成 |
| Query | 探索 |
| Transform | 変換（Resize） |

---

## 5. 全体構成まとめ

```
[Geometry]
    座標変換

[Topology]
    ├ Generator（Build）
    ├ Query（Face特定）
    └ Transform（Resize）

[Pattern]
    インスタンス保持
    Resizeの入口

[Editor]
    Geometry + Topologyを直接使用
```

---

## 6. 設計指針

### 6.1 禁止事項

- Patternに幾何ロジックを持たせない
- GeometryにTopology知識を持たせない
- GeneratorにQuery/Transformを混在させない

---

### 6.2 推奨事項

- Topologyドメイン内で責務を細分化
- Patternはデータコンテナとして保つ
- Editorはドメインサービスを直接利用する

---

## 7. 補足

- HitTestの具体的アルゴリズムは別途設計対象とする
- 将来的に空間インデックス導入可能
- ResizeアルゴリズムはStrategyとして差し替え可能
