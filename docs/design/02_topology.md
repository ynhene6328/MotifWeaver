# Topology設計

## 1. 概要

トポロジーはグラフ構造として定義される。

- Vertex
- Edge
- Face

---

## 2. VertexKey

```csharp
public readonly struct VertexKey
{
    public int X { get; }
    public int Y { get; }
}
```

---

## 3. EdgeKey

```csharp
public readonly struct EdgeKey
{
    public VertexKey A { get; }
    public VertexKey B { get; }
}
```

---

## 4. 重複排除

```csharp
Dictionary<VertexKey, Vertex> vertices;
Dictionary<EdgeKey, Edge> edges;
```

---

## 5. Face

```csharp
public class Face
{
    public IReadOnlyList<Vertex> Vertices { get; }
    public IReadOnlyList<Edge> Edges { get; }
    public Color Color { get; set; }
}
```

---

## 6. 設計原則

- Vertexは一意
- Edgeは順序無視で一意
- FaceはEdge集合

---