// /tests/MotifWeaver.Tests/Program.cs
using System;
using System.Collections.Generic;
using MotifWeaver.Core.Topology;

namespace MotifWeaver.Tests;

public static class Program
{
    public static int Main()
    {
        try
        {
            VertexUniquenessIsGuaranteed();
            EdgeUniquenessIsOrderInsensitive();
            EdgeFacesAreLimitedToTwo();
            FaceVerticesAreClockwise();
            AdjacentFacesShareEdge();

            Console.WriteLine("All topology tests passed.");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static void VertexUniquenessIsGuaranteed()
    {
        TopologyBuilder topologyBuilder = new TopologyBuilder();

        topologyBuilder.CreateFace(
        [
            new VertexKey(0, 0),
            new VertexKey(2, 0),
            new VertexKey(1, 1)
        ]);

        topologyBuilder.CreateFace(
        [
            new VertexKey(0, 0),
            new VertexKey(1, 1),
            new VertexKey(-1, 1)
        ]);

        AssertEqual(4, topologyBuilder.Vertices.Count, "Vertex uniqueness must be preserved.");
        AssertTrue(
            ReferenceEquals(
                topologyBuilder.Vertices[new VertexKey(0, 0)],
                topologyBuilder.Faces[0].Vertices[2]),
            "Shared vertices must reuse the same instance.");
    }

    private static void EdgeUniquenessIsOrderInsensitive()
    {
        TopologyBuilder topologyBuilder = new TopologyBuilder();

        topologyBuilder.CreateFace(
        [
            new VertexKey(0, 0),
            new VertexKey(2, 0),
            new VertexKey(1, 1)
        ]);

        Edge forwardEdge = topologyBuilder.Edges[new EdgeKey(new VertexKey(0, 0), new VertexKey(2, 0))];
        Edge reverseEdge = topologyBuilder.Edges[new EdgeKey(new VertexKey(2, 0), new VertexKey(0, 0))];

        AssertTrue(ReferenceEquals(forwardEdge, reverseEdge), "Edge uniqueness must ignore direction.");
        AssertEqual(3, topologyBuilder.Edges.Count, "A single triangle must create exactly three edges.");
    }

    private static void EdgeFacesAreLimitedToTwo()
    {
        TopologyBuilder topologyBuilder = new TopologyBuilder();

        topologyBuilder.CreateFace(
        [
            new VertexKey(0, 0),
            new VertexKey(2, 0),
            new VertexKey(1, 1)
        ]);

        topologyBuilder.CreateFace(
        [
            new VertexKey(2, 0),
            new VertexKey(0, 0),
            new VertexKey(1, -1)
        ]);

        int faceCountBefore = topologyBuilder.Faces.Count;
        int edgeCountBefore = topologyBuilder.Edges.Count;
        int vertexCountBefore = topologyBuilder.Vertices.Count;

        AssertThrows<InvalidOperationException>(
            () => topologyBuilder.CreateFace(
            [
                new VertexKey(1, 2),
                new VertexKey(0, 0),
                new VertexKey(2, 0)
            ]),
            "An edge must not accept a third face.");

        AssertEqual(faceCountBefore, topologyBuilder.Faces.Count, "Failed face creation must not add faces.");
        AssertEqual(edgeCountBefore, topologyBuilder.Edges.Count, "Failed face creation must not add edges.");
        AssertEqual(vertexCountBefore, topologyBuilder.Vertices.Count, "Failed face creation must not add vertices.");
    }

    private static void FaceVerticesAreClockwise()
    {
        TopologyBuilder topologyBuilder = new TopologyBuilder();

        Face face = topologyBuilder.CreateFace(
        [
            new VertexKey(0, 0),
            new VertexKey(2, 0),
            new VertexKey(2, 2),
            new VertexKey(0, 2)
        ]);

        AssertTrue(ComputeSignedArea(face.Vertices) < 0, "Face vertices must be normalized to clockwise order.");
    }

    private static void AdjacentFacesShareEdge()
    {
        TopologyBuilder topologyBuilder = new TopologyBuilder();

        Face firstFace = topologyBuilder.CreateFace(
        [
            new VertexKey(0, 0),
            new VertexKey(2, 0),
            new VertexKey(1, 1)
        ]);

        Face secondFace = topologyBuilder.CreateFace(
        [
            new VertexKey(2, 0),
            new VertexKey(0, 0),
            new VertexKey(1, -1)
        ]);

        Edge sharedEdge = topologyBuilder.Edges[new EdgeKey(new VertexKey(0, 0), new VertexKey(2, 0))];

        AssertTrue(firstFace.Edges.Contains(sharedEdge), "The first face must reference the shared edge.");
        AssertTrue(secondFace.Edges.Contains(sharedEdge), "The second face must reference the shared edge.");
        AssertEqual(2, sharedEdge.Faces.Count, "The shared edge must reference exactly two faces.");
    }

    private static long ComputeSignedArea(IReadOnlyList<Vertex> vertices)
    {
        long twiceArea = 0;

        for (int index = 0; index < vertices.Count; index++)
        {
            VertexKey current = vertices[index].Key;
            VertexKey next = vertices[(index + 1) % vertices.Count].Key;
            twiceArea += ((long)current.X * next.Y) - ((long)next.X * current.Y);
        }

        return twiceArea;
    }

    private static void AssertTrue(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void AssertEqual<T>(T expected, T actual, string message)
        where T : IEquatable<T>
    {
        if (!expected.Equals(actual))
        {
            throw new InvalidOperationException($"{message} Expected: {expected}. Actual: {actual}.");
        }
    }

    private static void AssertThrows<TException>(Action action, string message)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException(message);
    }
}
