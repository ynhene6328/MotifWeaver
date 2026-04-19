# 14. HitTest設計

## 1. 目的

本ドキュメントでは以下を定義する：

- ユーザクリックからFace特定までの処理フロー
- HitTestの基本アルゴリズム
- 将来的な最適化方針
- TopologyQueryインターフェース設計

---

## 2. 前提

- Faceは三角形または六角形（凸多角形）
- 描画はGeometryにより論理座標→描画座標へ変換される
- HitTestはその逆（描画座標→論理座標）を利用する

---

## 3. 処理フロー

```
[1] ユーザクリック（screen座標）
    ↓
[2] Geometry
    screen → logical（実数座標）
    ↓
[3] TopologyQuery
    logical → Face特定
    ↓
[4] Editor
    Faceに対する操作
```

---

## 4. 基本アルゴリズム

### 4.1 方針

各FaceをPolygonとして扱い、点がその内部に存在するか判定する。

---

### 4.2 採用アルゴリズム

**Ray Casting（レイキャスティング法）**

#### 概要

- 点から右方向に半直線を引く
- Polygonの辺との交差回数を数える
- 交差回数が奇数 → 内部
- 偶数 → 外部

---

### 4.3 擬似コード

```
bool IsPointInPolygon(point, polygon):
    inside = false

    for each edge (vi, vj):
        if edge crosses horizontal ray from point:
            inside = !inside

    return inside
```

---

### 4.4 実装上の注意

#### ① 境界上の扱い

- 境界付近の挙動は未定義とする
- 最初にヒットしたFaceを採用する

---

#### ② 浮動小数点誤差

- 特別な対策は行わない
- 通常の比較演算で判定する

---

#### ③ パフォーマンス

- 全Faceを対象とするため O(N)
- Face数増加で性能低下

---

## 5. 初期最適化（必須）

### 5.1 BoundingBoxフィルタ

各Faceに対して以下を事前計算する：

```
BoundingBox (minX, minY, maxX, maxY)
```

#### 判定フロー

```
if point が BoundingBox内でない:
    → Polygon判定をスキップ
```

---

## 6. 将来最適化

### 6.1 空間インデックス

#### 概要

空間を分割し、位置から候補Faceを高速に特定する仕組み

---

### 6.2 近傍探索（推奨）

#### 概要

クリック位置から対応する格子位置を逆算する

---

#### フロー

```
[1] screen → logical
[2] logical → 近傍セル推定
[3] 周囲Faceのみ判定
```

---

## 7. アルゴリズム比較

| 手法 | 計算量 | 実装難易度 | 備考 |
|------|--------|------------|------|
| 全探索 + Ray Casting | O(N) | 低 | 初期実装 |
| BoundingBoxフィルタ | O(N) | 低 | 必須 |
| 空間インデックス | O(logN)〜O(1) | 中 | 汎用 |
| 近傍探索 | O(1) | 中 | 最適 |

---

## 8. 採用方針

### Phase 1（現時点）

- Ray Casting
- BoundingBoxフィルタ

---

### Phase 2（将来）

- 近傍探索ベースへ移行

---

## 9. TopologyQueryインターフェース設計

### 9.1 目的

- 論理座標からFaceを特定する
- HitTestロジックをTopologyドメインに分離する
- アルゴリズムの差し替えを可能にする

---

### 9.2 インターフェース

```
public interface ITopologyQuery
{
    Face? FindFace(
        IReadOnlyList<Face> faces,
        Vector2 logicalPosition,
        IGridGeometry geometry);
}
```

---

### 9.3 設計意図

#### 入力

- faces
  - Patternが保持するFace集合
- logicalPosition
  - Geometryで変換済みの論理座標
- geometry
  - VertexKey → Vector2変換に使用

---

#### 出力

- 条件に一致するFace
- 見つからない場合は null

---

### 9.4 実装方針

初期実装：

```
class RayCastingTopologyQuery : ITopologyQuery
```

処理内容：

```
for each Face:
    if BoundingBox外:
        continue

    polygon = VertexKey → Vector2変換

    if PointInPolygon:
        return Face

return null
```

---

### 9.5 将来拡張

以下の差し替えが可能：

- 近傍探索版
- 空間インデックス版

---

## 10. 設計指針

- HitTestロジックはTopologyドメインに配置する
- Geometryは座標変換のみに限定する
- Patternは探索ロジックを持たない
- Queryはアルゴリズム差し替え可能とする

---

## 11. 補足

- Triangle / Hex は凸多角形のため判定が安定
- 将来的に専用最適化（近傍探索）へ移行予定
