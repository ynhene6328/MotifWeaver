// /tests/MotifWeaver.Tests/Program.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MotifWeaver.Core.Geometry;
using MotifWeaver.Core.Topology;
using MotifWeaver.Rendering;

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
            NeighborsAreDerivedFromSharedEdges();
            HexGridSharesEdgesAndReusesVertices();

            HexGeometryReturnsSamePositionForSameKey();
            HexGeometryReturnsDifferentPositionsForDifferentKeys();
            ComputeBoundsReturnsCorrectRange();
            BoundingBoxWidthAndHeightAreCorrect();
            CachedResultsAreStable();

            RenderServiceCallsDrawPolygonOnceForSingleFace();
            RenderServicePassesCorrectVertexCount();
            RenderServiceUsesGeometryTransformedCoordinates();
            RenderServicePreservesVertexOrder();
            RenderServiceCallsDrawPolygonForEachFace();
            RenderServiceHandlesEmptyCollection();

            Console.WriteLine("All tests passed.");
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

    private static void NeighborsAreDerivedFromSharedEdges()
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

        Face isolatedFace = topologyBuilder.CreateFace(
        [
            new VertexKey(10, 10),
            new VertexKey(12, 10),
            new VertexKey(11, 11)
        ]);

        List<Face> firstNeighbors = new List<Face>(topologyBuilder.GetNeighbors(firstFace));
        List<Face> secondNeighbors = new List<Face>(topologyBuilder.GetNeighbors(secondFace));
        List<Face> isolatedNeighbors = new List<Face>(topologyBuilder.GetNeighbors(isolatedFace));

        AssertEqual(1, firstNeighbors.Count, "A shared edge must produce one adjacent face.");
        AssertEqual(1, secondNeighbors.Count, "Adjacent faces must be discovered symmetrically.");
        AssertEqual(0, isolatedNeighbors.Count, "A face without shared edges must have no neighbors.");
        AssertTrue(ReferenceEquals(secondFace, firstNeighbors[0]), "Neighbor lookup must return the face sharing the edge.");
        AssertTrue(ReferenceEquals(firstFace, secondNeighbors[0]), "Neighbor lookup must return the shared adjacent face.");
    }

    private static void HexGridSharesEdgesAndReusesVertices()
    {
        HexGridTopology hexGridTopology = new HexGridTopology();
        IReadOnlyList<Face> faces = hexGridTopology.Build(2, 2);
        TopologyBuilder topologyBuilder = hexGridTopology.Builder;

        AssertEqual(4, faces.Count, "A 2x2 hex grid must create four faces.");
        AssertEqual(16, topologyBuilder.Vertices.Count, "Hex grid vertices must be reused instead of duplicated.");

        int sharedEdgeCount = 0;

        foreach (Edge edge in topologyBuilder.Edges.Values)
        {
            AssertTrue(edge.Faces.Count <= 2, "Every edge must reference at most two faces.");

            if (edge.Faces.Count == 2)
            {
                sharedEdgeCount++;
            }
        }

        AssertEqual(5, sharedEdgeCount, "The 2x2 hex grid must contain one shared interior edge.");

        List<Face> neighborsOfUpperRight = new List<Face>(topologyBuilder.GetNeighbors(faces[1]));
        List<Face> neighborsOfLowerLeft = new List<Face>(topologyBuilder.GetNeighbors(faces[2]));

        AssertEqual(3, neighborsOfUpperRight.Count, "The upper-right hex must have three adjacent hexes in a 2x2 grid.");
        AssertEqual(3, neighborsOfLowerLeft.Count, "The lower-left hex must have three adjacent hexes in a 2x2 grid.");
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

    private static void AssertNearlyEqual(float expected, float actual, string message, float tolerance = 0.0001f)
    {
        if (MathF.Abs(expected - actual) > tolerance)
        {
            throw new InvalidOperationException($"{message} Expected: {expected}. Actual: {actual}.");
        }
    }

    // --- Geometryテスト ---

    private static void HexGeometryReturnsSamePositionForSameKey()
    {
        HexGridGeometry geometry = new HexGridGeometry(1.0f);
        VertexKey key = new VertexKey(3, 2);

        Vector2 first = geometry.GetPosition(key);
        Vector2 second = geometry.GetPosition(key);

        AssertNearlyEqual(first.X, second.X, "同一キーから得られるX座標は一致しなければならない。");
        AssertNearlyEqual(first.Y, second.Y, "同一キーから得られるY座標は一致しなければならない。");
    }

    private static void HexGeometryReturnsDifferentPositionsForDifferentKeys()
    {
        HexGridGeometry geometry = new HexGridGeometry(1.0f);
        VertexKey keyA = new VertexKey(0, 0);
        VertexKey keyB = new VertexKey(1, 0);
        VertexKey keyC = new VertexKey(0, 1);

        Vector2 posA = geometry.GetPosition(keyA);
        Vector2 posB = geometry.GetPosition(keyB);
        Vector2 posC = geometry.GetPosition(keyC);

        AssertTrue(
            MathF.Abs(posA.X - posB.X) > 0.0001f || MathF.Abs(posA.Y - posB.Y) > 0.0001f,
            "異なるVertexKeyは異なる座標を返さなければならない。(A vs B)");
        AssertTrue(
            MathF.Abs(posA.X - posC.X) > 0.0001f || MathF.Abs(posA.Y - posC.Y) > 0.0001f,
            "異なるVertexKeyは異なる座標を返さなければならない。(A vs C)");
    }

    private static void ComputeBoundsReturnsCorrectRange()
    {
        HexGridGeometry geometry = new HexGridGeometry(1.0f);

        // 六角形(q=0,r=0)の6頂点: (1,0),(0,1),(-1,1),(-1,0),(0,-1),(1,-1)
        List<VertexKey> keys =
        [
            new VertexKey(1, 0),
            new VertexKey(0, 1),
            new VertexKey(-1, 1),
            new VertexKey(-1, 0),
            new VertexKey(0, -1),
            new VertexKey(1, -1)
        ];

        BoundingBox bounds = geometry.ComputeBounds(keys);

        // x = X * (w/2): 最小X=-1 → -0.5, 最大X=1 → 0.5
        AssertNearlyEqual(-0.5f, bounds.MinX, "ComputeBoundsのMinXが正しくなければならない。");
        AssertNearlyEqual(0.5f, bounds.MaxX, "ComputeBoundsのMaxXが正しくなければならない。");

        // y = Y * (√3/2) * w: 最小Y=-1 → -√3/2, 最大Y=1 → √3/2
        float halfSqrt3 = MathF.Sqrt(3.0f) / 2.0f;
        AssertNearlyEqual(-halfSqrt3, bounds.MinY, "ComputeBoundsのMinYが正しくなければならない。");
        AssertNearlyEqual(halfSqrt3, bounds.MaxY, "ComputeBoundsのMaxYが正しくなければならない。");
    }

    private static void BoundingBoxWidthAndHeightAreCorrect()
    {
        HexGridGeometry geometry = new HexGridGeometry(2.0f);

        // unitSize=2.0の場合: x = X * 1.0, y = Y * √3
        List<VertexKey> keys =
        [
            new VertexKey(1, 0),
            new VertexKey(0, 1),
            new VertexKey(-1, 1),
            new VertexKey(-1, 0),
            new VertexKey(0, -1),
            new VertexKey(1, -1)
        ];

        BoundingBox bounds = geometry.ComputeBounds(keys);

        // Width = MaxX - MinX = 1.0 - (-1.0) = 2.0
        AssertNearlyEqual(2.0f, bounds.Width, "BoundingBoxのWidthが正しくなければならない。");

        // Height = MaxY - MinY = √3 - (-√3) = 2√3
        float expectedHeight = MathF.Sqrt(3.0f) * 2.0f;
        AssertNearlyEqual(expectedHeight, bounds.Height, "BoundingBoxのHeightが正しくなければならない。");
    }

    private static void CachedResultsAreStable()
    {
        HexGridGeometry geometry = new HexGridGeometry(1.0f);
        VertexKey key = new VertexKey(5, 3);

        // 初回呼び出し（キャッシュ未登録）
        Vector2 first = geometry.GetPosition(key);

        // 2回目呼び出し（キャッシュヒット）
        Vector2 second = geometry.GetPosition(key);

        // 3回目呼び出し（キャッシュヒット）
        Vector2 third = geometry.GetPosition(key);

        AssertNearlyEqual(first.X, second.X, "キャッシュ後もX座標が安定しなければならない。");
        AssertNearlyEqual(first.Y, second.Y, "キャッシュ後もY座標が安定しなければならない。");
        AssertNearlyEqual(second.X, third.X, "複数回のキャッシュヒットでもX座標が安定しなければならない。");
        AssertNearlyEqual(second.Y, third.Y, "複数回のキャッシュヒットでもY座標が安定しなければならない。");
    }

    // --- RenderServiceテスト ---

    private static void RenderServiceCallsDrawPolygonOnceForSingleFace()
    {
        TopologyBuilder builder = new TopologyBuilder();
        Face face = builder.CreateFace(
        [
            new VertexKey(0, 0),
            new VertexKey(2, 0),
            new VertexKey(1, 1)
        ]);

        HexGridGeometry geometry = new HexGridGeometry(1.0f);
        MockRenderer renderer = new MockRenderer();
        MotifWeaver.Rendering.UseCases.EditorRenderer service = new MotifWeaver.Rendering.UseCases.EditorRenderer(renderer, geometry, new Func<int, MotifWeaver.Rendering.Color>(id => new MotifWeaver.Rendering.Color(200, 0, 0)));

        service.Render(new Pattern(new MockGridTopology([face]), 4, 4));

        AssertEqual(1, renderer.DrawnPolygons.Count, "Face1つに対しDrawPolygonが1回呼ばれなければならない。");
    }

    private static void RenderServicePassesCorrectVertexCount()
    {
        TopologyBuilder builder = new TopologyBuilder();
        Face triangle = builder.CreateFace(
        [
            new VertexKey(0, 0),
            new VertexKey(2, 0),
            new VertexKey(1, 1)
        ]);

        HexGridGeometry geometry = new HexGridGeometry(1.0f);
        MockRenderer renderer = new MockRenderer();
        MotifWeaver.Rendering.UseCases.EditorRenderer service = new MotifWeaver.Rendering.UseCases.EditorRenderer(renderer, geometry, new Func<int, MotifWeaver.Rendering.Color>(id => new MotifWeaver.Rendering.Color(200, 0, 0)));

        service.Render(new Pattern(new MockGridTopology([triangle]), 4, 4));

        AssertEqual(
            triangle.Vertices.Count,
            renderer.DrawnPolygons[0].Points.Count,
            "渡されるVector2の数はFaceの頂点数と一致しなければならない。");
    }

    private static void RenderServiceUsesGeometryTransformedCoordinates()
    {
        TopologyBuilder builder = new TopologyBuilder();
        Face face = builder.CreateFace(
        [
            new VertexKey(1, 0),
            new VertexKey(0, 1),
            new VertexKey(-1, 1)
        ]);

        HexGridGeometry geometry = new HexGridGeometry(1.0f);
        MockRenderer renderer = new MockRenderer();
        MotifWeaver.Rendering.UseCases.EditorRenderer service = new MotifWeaver.Rendering.UseCases.EditorRenderer(renderer, geometry, new Func<int, MotifWeaver.Rendering.Color>(id => new MotifWeaver.Rendering.Color(200, 0, 0)));

        service.Render(new Pattern(new MockGridTopology([face]), 4, 4));

        IReadOnlyList<Vector2> points = renderer.DrawnPolygons[0].Points;

        // Geometry変換後の座標と一致することを確認
        for (int index = 0; index < face.Vertices.Count; index++)
        {
            Vector2 expected = geometry.GetPosition(face.Vertices[index].Key);
            AssertNearlyEqual(expected.X, points[index].X,
                "Geometry経由のX座標が使われていなければならない。");
            AssertNearlyEqual(expected.Y, points[index].Y,
                "Geometry経由のY座標が使われていなければならない。");
        }

        // 整数座標そのままではないことを確認（少なくとも1つの頂点で差がある）
        bool anyDifference = false;
        for (int index = 0; index < face.Vertices.Count; index++)
        {
            VertexKey key = face.Vertices[index].Key;
            Vector2 point = points[index];
            if (MathF.Abs(point.X - key.X) > 0.0001f || MathF.Abs(point.Y - key.Y) > 0.0001f)
            {
                anyDifference = true;
                break;
            }
        }

        AssertTrue(anyDifference, "座標はGeometryで変換されたものでなければならない（整数座標そのままは不可）。");
    }

    private static void RenderServicePreservesVertexOrder()
    {
        TopologyBuilder builder = new TopologyBuilder();
        Face face = builder.CreateFace(
        [
            new VertexKey(0, 0),
            new VertexKey(2, 0),
            new VertexKey(1, 1)
        ]);

        HexGridGeometry geometry = new HexGridGeometry(1.0f);
        MockRenderer renderer = new MockRenderer();
        MotifWeaver.Rendering.UseCases.EditorRenderer service = new MotifWeaver.Rendering.UseCases.EditorRenderer(renderer, geometry, new Func<int, MotifWeaver.Rendering.Color>(id => new MotifWeaver.Rendering.Color(200, 0, 0)));

        service.Render(new Pattern(new MockGridTopology([face]), 4, 4));

        IReadOnlyList<Vector2> points = renderer.DrawnPolygons[0].Points;

        // Faceの頂点順序と同じ順序でVector2が渡されることを確認
        for (int index = 0; index < face.Vertices.Count; index++)
        {
            Vector2 expected = geometry.GetPosition(face.Vertices[index].Key);
            AssertNearlyEqual(expected.X, points[index].X,
                $"頂点{index}のX座標の順序が維持されていなければならない。");
            AssertNearlyEqual(expected.Y, points[index].Y,
                $"頂点{index}のY座標の順序が維持されていなければならない。");
        }
    }

    private static void RenderServiceCallsDrawPolygonForEachFace()
    {
        HexGridTopology hexGrid = new HexGridTopology();
        Pattern pattern = new Pattern(hexGrid, 2, 2);

        HexGridGeometry geometry = new HexGridGeometry(1.0f);
        MockRenderer renderer = new MockRenderer();
        MotifWeaver.Rendering.UseCases.EditorRenderer service = new MotifWeaver.Rendering.UseCases.EditorRenderer(renderer, geometry, new Func<int, MotifWeaver.Rendering.Color>(id => new MotifWeaver.Rendering.Color(200, 0, 0)));

        service.Render(pattern);

        AssertEqual(pattern.Faces.Count, renderer.DrawnPolygons.Count,
            "Face数と同じ回数だけDrawPolygonが呼ばれなければならない。");
    }

    private static void RenderServiceHandlesEmptyCollection()
    {
        HexGridGeometry geometry = new HexGridGeometry(1.0f);
        MockRenderer renderer = new MockRenderer();
        MotifWeaver.Rendering.UseCases.EditorRenderer service = new MotifWeaver.Rendering.UseCases.EditorRenderer(renderer, geometry, new Func<int, MotifWeaver.Rendering.Color>(id => new MotifWeaver.Rendering.Color(200, 0, 0)));

        // 例外が出ないことを確認
        service.Render(new Pattern(new MockGridTopology(new List<Face>()), 4, 4));

        AssertEqual(0, renderer.DrawnPolygons.Count,
            "空コレクションではDrawPolygonが呼ばれてはならない。");
        AssertTrue(renderer.BeginCalled, "空コレクションでもBeginは呼ばれなければならない。");
        AssertTrue(renderer.EndCalled, "空コレクションでもEndは呼ばれなければならない。");
    }
}

/// <summary>
/// テスト用のIGridTopology実装。直接Faceリストを返す。
/// </summary>
internal sealed class MockGridTopology : IGridTopology
{
    public int UnitX => 1;
    public int UnitY => 1;
    public int UnitRow => 1;
    public int UnitCol => 1;
    public int Row => 1;
    public int Col => 1;
    public int LogicalWidth => 1;
    public int LogicalHeight => 1;
    public int MaxWidth => 1;
    public int MaxHeight => 1;
    private readonly IReadOnlyList<Face> _faces;

    public MockGridTopology(IReadOnlyList<Face> faces)
    {
        _faces = faces;
    }

    public IReadOnlyList<Face> Build(int rows, int cols)
    {
        return _faces;
    }
    public IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0)
    {
        return _faces;
    }
    public (int row, int col) GetGridFaceRowCol(Face face)
    {
        return (0, 0);
    }
}

/// <summary>
/// テスト用のIRenderer実装。DrawPolygonの呼び出し内容を記録する。
/// </summary>
internal sealed class MockRenderer : IRenderer
{
    private readonly List<DrawnPolygon> _drawnPolygons;

    public MockRenderer()
    {
        _drawnPolygons = new List<DrawnPolygon>();
    }

    public bool BeginCalled { get; private set; }

    public bool EndCalled { get; private set; }

    public IReadOnlyList<DrawnPolygon> DrawnPolygons => _drawnPolygons;

    public void Begin()
    {
        BeginCalled = true;
    }

    public void DrawPolygon(IReadOnlyList<Vector2> points, Color fillColor)
    {
        _drawnPolygons.Add(new DrawnPolygon(
            new List<Vector2>(points),
            fillColor));
    }

    public void End()
    {
        EndCalled = true;
    }
    public void SetCanvasSize(int width, int height)
    {
        // テストではキャンバスサイズは無視する
    }
}

/// <summary>
/// DrawPolygonに渡された内容を保持するレコード
/// </summary>
internal sealed class DrawnPolygon
{
    public DrawnPolygon(IReadOnlyList<Vector2> points, Color fillColor)
    {
        Points = points;
        FillColor = fillColor;
    }

    public IReadOnlyList<Vector2> Points { get; }

    public Color FillColor { get; }
}
