// /src/App/Program.cs
using System;
using System.Collections.Generic;
using MotifWeaver.Topology;

namespace MotifWeaver.App;

public static class Program
{
    public static void Main()
    {
        TopologyBuilder topologyBuilder = new TopologyBuilder();
        IReadOnlyList<VertexKey> faceVertices =
        [
            new VertexKey(0, 0),
            new VertexKey(2, 0),
            new VertexKey(1, 1)
        ];

        Face face = topologyBuilder.CreateFace(faceVertices);

        Console.WriteLine(
            $"Faces={topologyBuilder.Faces.Count}, Vertices={topologyBuilder.Vertices.Count}, Edges={topologyBuilder.Edges.Count}, FaceVertices={face.Vertices.Count}");
    }
}
