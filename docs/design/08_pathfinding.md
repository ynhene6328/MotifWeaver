# Pathfinding設計

## 1. 目的

同色領域を一筆書き可能な経路として抽出する。

---

## 2. グラフ定義

- ノード：Edge
- 接続：Vertex共有

---

## 3. サブグラフ抽出

```csharp
edges = allEdges.Where(e => e.Color == targetColor)
```

---

## 4. 隣接定義

```csharp
IEnumerable<Edge> GetAdjacentEdges(Edge e)
{
    foreach (var v in [e.V1, e.V2])
        foreach (var edge in v.Edges)
            if (edge != e)
                yield return edge;
}
```

---

## 5. オイラー路判定

条件：

```text
奇数次数の頂点が0または2
```

---

## 6. 経路生成（Hierholzer）

```csharp
List<Edge> FindEulerPath(Graph g)
{
    stack = new Stack<Edge>();
    path = new List<Edge>();

    while (stack not empty)
    {
        if (current has unused edge)
            stack.push(edge)
        else
            path.add(stack.pop())
    }
}
```

---

## 7. 分割処理

- 連結成分ごとに処理

---

## 8. 出力

```csharp
List<List<Edge>> paths;
```

---

## 9. 将来拡張

- 重み付き経路
- 最短経路
- 編み順最適化

---