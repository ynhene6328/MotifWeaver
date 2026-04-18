# Pattern と Attribute 設計（改訂版）

## 1. 概要

本設計では、編み物モチーフ（六角形・三角形など）を単位としたパターン編集および描画を行う。

アプリケーションの主目的：

- Editor上で Pattern（複数Face）に属性（色など）を設定する
- Viewer上で Pattern を繰り返し描画する
- 属性変更が即座に全体へ反映される

---

## 2. 用語定義

| 用語 | 説明 |
|------|------|
| Topology | Vertex / Edge / Face の接続構造 |
| Geometry | 論理座標 → 描画座標変換 |
| Pattern | 複数Faceから構成される編集単位 |
| AttributeId | 要素に付与される抽象的なID（意味は持たない） |
| Palette | AttributeId → Color の対応表（UI層） |
| Viewer | Patternを繰り返し描画する表示専用領域 |
| Editor | Patternを編集するUI |

---

## 3. 基本方針

### 3.1 責務分離

| レイヤ | 責務 |
|--------|------|
| Topology | 構造 + AttributeId |
| Geometry | 座標変換 |
| Rendering | 描画API抽象 |
| Pattern | 編集対象の集合 |
| UI | 状態管理・属性の意味付け |

---

## 4. Attribute設計

### 4.1 方針

```text
AttributeIdは「意味を持たない識別子」
```

- 色に限定しない
- 将来拡張（グループ・選択状態など）を前提とする

---

### 4.2 interface定義

```csharp
public interface IAttributeHolder
{
    int AttributeId { get; set; }
}
```

---

### 4.3 適用対象

```csharp
public sealed class Face : IAttributeHolder
{
    public int AttributeId { get; set; }
}

public sealed class Edge : IAttributeHolder
{
    public int AttributeId { get; set; }
}
```

- Vertexは対象外（必須ではない）

---

### 4.4 設計意図

```text
構造（Topology）と意味（UI）を分離する
```

---

## 5. Pattern設計

### 5.1 配置

```text
MotifWeaver.Core に配置する
```

理由：

- UI非依存
- Blazor / WPF で共通利用
- 純粋なドメイン概念

---

### 5.2 定義

```csharp
public sealed class Pattern
{
    public IReadOnlyList<Face> Faces { get; }

    public int Rows { get; }
    public int Cols { get; }

    public Pattern(IReadOnlyList<Face> faces, int rows, int cols)
    {
        if (cols % 2 != 0)
        {
            throw new ArgumentException("列数は偶数である必要があります");
        }

        Faces = faces;
        Rows = rows;
        Cols = cols;
    }
}
```

---

### 5.3 サイズ制約

```text
列方向は必ず偶数
行方向は制約なし
```

---

### 5.4 最小周期

| 形状 | 最小単位 |
|------|----------|
| 三角形 | 1行2列 |
| 六角形 | 1行2列 |

---

## 6. Patternの繰り返し（Viewer）

### 6.1 方針

```text
Viewerは完全に描画専用
```

- Patternインスタンスは保持しない
- Polygonを描画するだけ

---

### 6.2 描画方法

```text
Patternを平行移動して描画する
```

---

### 6.3 重要仕様

```text
オフセット補正は不要
```

理由：

- 偶数列制約により接続が保証される
- 境界の凹凸は自然に噛み合う

---

## 7. Palette設計（UI層）

### 7.1 配置

```text
Coreには含めない
UI層で管理する
```

---

### 7.2 定義例

```csharp
public sealed class Palette
{
    private readonly Dictionary<int, Color> _colors;
}
```

---

### 7.3 役割

```text
AttributeId → Color に変換する
```

---

### 7.4 特徴

- AttributeId変更で全体が即時反映
- Coreは色の概念を持たない

---

## 8. ViewModel設計（WPF）

### 8.1 方針

```text
ViewModelはPatternを保持する
```

---

### 8.2 例

```csharp
public sealed class MainViewModel
{
    public Pattern Pattern { get; }

    public Dictionary<int, Color> Palette { get; }

    public Face? SelectedFace { get; set; }

    public int SelectedAttributeId { get; set; }

    public void ApplyAttribute()
    {
        if (SelectedFace != null)
        {
            SelectedFace.AttributeId = SelectedAttributeId;
        }
    }
}
```

---

### 8.3 非推奨

```text
ViewModelがFace一覧を直接管理する
```

理由：

- ドメイン構造の破壊
- 拡張性低下

---

## 9. WPF と Blazor の関係

### 9.1 WPF

```text
MVVM構成
```

---

### 9.2 Blazor

```text
Componentベース（MVVMではない）
```

---

### 9.3 共通化方針

#### 共通

- Pattern
- Topology
- Geometry
- AttributeId

#### 非共通

- UI状態管理
- Palette保持方法

---

## 10. データフロー

```text
[Editor]
  ↓
Pattern（AttributeId更新）
  ↓
RenderService
  ↓
IRenderer
  ↓
Viewer描画
```

---

## 11. 設計の核心

```text
AttributeIdは意味を持たない
意味はUIが与える
```

```text
Patternは構造
Paletteは表現
```

---

## 12. 今後の検討事項

- HitTest（座標 → Face特定）
- Pattern保存形式
- 属性の種類拡張
- Geometry回転対応
- Viewer最適化

---

## 13. まとめ

本設計の特徴：

- 構造と意味の完全分離
- UI非依存なCore設計
- 高い拡張性
- WPF / Blazor共通化可能
