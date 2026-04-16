// /src/Topology/TopologyBuilder.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Topology;

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

        List<Vertex> vertices = new List<Vertex>(vertexKeys.Count);
        List<Edge> edges = new List<Edge>(vertexKeys.Count);

        for (int index = 0; index < vertexKeys.Count; index++)
        {
            Vertex vertex = GetOrCreateVertex(vertexKeys[index]);
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

    public Vertex GetOrCreateVertex(VertexKey key)
    {
        if (_vertexMap.TryGetValue(key, out Vertex? existingVertex))
        {
            return existingVertex;
        }

        Vertex vertex = new Vertex(key);
        _vertexMap.Add(key, vertex);
        return vertex;
    }

    public Edge GetOrCreateEdge(Vertex first, Vertex second)
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
}
