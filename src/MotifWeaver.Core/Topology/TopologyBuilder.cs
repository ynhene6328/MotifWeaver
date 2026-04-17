// /src/MotifWeaver.Core/Topology/TopologyBuilder.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class TopologyBuilder
{
    private readonly Dictionary<VertexKey, Vertex> _vertexMap;
    private readonly Dictionary<EdgeKey, Edge> _edgeMap;
    private readonly List<Face> _faces;

    public TopologyBuilder()
    {
        _vertexMap = new Dictionary<VertexKey, Vertex>();
        _edgeMap = new Dictionary<EdgeKey, Edge>();
        _faces = new List<Face>();
    }

    public IReadOnlyDictionary<VertexKey, Vertex> Vertices => _vertexMap;

    public IReadOnlyDictionary<EdgeKey, Edge> Edges => _edgeMap;

    public IReadOnlyList<Face> Faces => _faces;

    public Face CreateFace(IReadOnlyList<VertexKey> vertexKeys)
    {
        if (vertexKeys is null)
        {
            throw new ArgumentNullException(nameof(vertexKeys));
        }

        if (vertexKeys.Count < 3)
        {
            throw new ArgumentException("A face requires at least three vertices.", nameof(vertexKeys));
        }

        List<VertexKey> normalizedVertexKeys = NormalizeClockwise(vertexKeys);
        ValidateFace(normalizedVertexKeys);

        List<Vertex> vertices = new List<Vertex>(normalizedVertexKeys.Count);
        List<Edge> edges = new List<Edge>(normalizedVertexKeys.Count);

        for (int index = 0; index < normalizedVertexKeys.Count; index++)
        {
            Vertex vertex = GetOrCreateVertex(normalizedVertexKeys[index]);
            vertices.Add(vertex);
        }

        for (int index = 0; index < vertices.Count; index++)
        {
            Vertex current = vertices[index];
            Vertex next = vertices[(index + 1) % vertices.Count];
            Edge edge = GetOrCreateEdge(current, next);
            edges.Add(edge);
        }

        Face face = new Face(vertices, edges);

        for (int index = 0; index < edges.Count; index++)
        {
            edges[index].AttachFace(face);
        }

        _faces.Add(face);
        return face;
    }

    private Vertex GetOrCreateVertex(VertexKey key)
    {
        if (_vertexMap.TryGetValue(key, out Vertex? existingVertex))
        {
            return existingVertex;
        }

        Vertex vertex = new Vertex(key);
        _vertexMap.Add(key, vertex);
        return vertex;
    }

    private Edge GetOrCreateEdge(Vertex first, Vertex second)
    {
        if (first is null)
        {
            throw new ArgumentNullException(nameof(first));
        }

        if (second is null)
        {
            throw new ArgumentNullException(nameof(second));
        }

        EdgeKey key = new EdgeKey(first.Key, second.Key);
        if (_edgeMap.TryGetValue(key, out Edge? existingEdge))
        {
            return existingEdge;
        }

        Edge edge = new Edge(first, second);
        _edgeMap.Add(key, edge);

        first.AttachEdge(edge);
        second.AttachEdge(edge);

        return edge;
    }

    private void ValidateFace(IReadOnlyList<VertexKey> vertexKeys)
    {
        HashSet<VertexKey> uniqueVertices = new HashSet<VertexKey>();
        HashSet<EdgeKey> uniqueEdges = new HashSet<EdgeKey>();

        for (int index = 0; index < vertexKeys.Count; index++)
        {
            VertexKey current = vertexKeys[index];
            VertexKey next = vertexKeys[(index + 1) % vertexKeys.Count];

            if (!uniqueVertices.Add(current))
            {
                throw new ArgumentException("A face cannot contain duplicated vertices.", nameof(vertexKeys));
            }

            EdgeKey edgeKey = new EdgeKey(current, next);
            if (!uniqueEdges.Add(edgeKey))
            {
                throw new ArgumentException("A face cannot contain duplicated edges.", nameof(vertexKeys));
            }

            if (_edgeMap.TryGetValue(edgeKey, out Edge? existingEdge) && existingEdge.Faces.Count >= 2)
            {
                throw new InvalidOperationException("An edge cannot reference more than two faces.");
            }
        }
    }

    private static List<VertexKey> NormalizeClockwise(IReadOnlyList<VertexKey> vertexKeys)
    {
        List<VertexKey> normalized = new List<VertexKey>(vertexKeys.Count);

        for (int index = 0; index < vertexKeys.Count; index++)
        {
            normalized.Add(vertexKeys[index]);
        }

        long signedArea = ComputeSignedArea(normalized);
        if (signedArea == 0)
        {
            throw new ArgumentException("A face requires a non-degenerate polygon.", nameof(vertexKeys));
        }

        if (signedArea > 0)
        {
            normalized.Reverse();
        }

        return normalized;
    }

    private static long ComputeSignedArea(IReadOnlyList<VertexKey> vertexKeys)
    {
        long twiceArea = 0;

        for (int index = 0; index < vertexKeys.Count; index++)
        {
            VertexKey current = vertexKeys[index];
            VertexKey next = vertexKeys[(index + 1) % vertexKeys.Count];
            twiceArea += ((long)current.X * next.Y) - ((long)next.X * current.Y);
        }

        return twiceArea;
    }
}
