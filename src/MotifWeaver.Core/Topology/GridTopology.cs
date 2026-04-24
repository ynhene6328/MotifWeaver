// /src/MotifWeaver.Core/Topology/GridTopology.cs
using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public abstract class GridTopology : IGridTopology
{
    public abstract int UnitX { get; }
    public abstract int UnitY { get; }
    public abstract int UnitRow { get; }
    public abstract int UnitCol { get; }
    public int MaxWidth => _topologyBuilder.Faces.Max(f => f.Vertices.Max(v => v.Key.X)) - _topologyBuilder.Faces.Min(f => f.Vertices.Min(v => v.Key.X));
    public int MaxHeight => _topologyBuilder.Faces.Max(f => f.Vertices.Max(v => v.Key.Y)) - _topologyBuilder.Faces.Min(f => f.Vertices.Min(v => v.Key.Y));
    public abstract int LogicalWidth { get; }
    public abstract int LogicalHeight { get; }
    public int Row => LogicalHeight / UnitY;
    public int Col => LogicalWidth / UnitX;

    protected TopologyBuilder _topologyBuilder;
    public GridTopology()
        : this(new TopologyBuilder())
    {
    }

    public GridTopology(TopologyBuilder topologyBuilder)
    {
        _topologyBuilder = topologyBuilder ?? throw new ArgumentNullException(nameof(topologyBuilder));
    }

    public TopologyBuilder Builder => _topologyBuilder;

    public IReadOnlyList<Face> Build(int rows, int cols)
    {
        if (rows < 0)
            throw new ArgumentOutOfRangeException(nameof(rows));

        if (cols < 0)
            throw new ArgumentOutOfRangeException(nameof(cols));

        List<Face> faces = new List<Face>(rows * cols);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var vertices = CreateGridFaceVertices(row, col);
                var face = _topologyBuilder.CreateFace(vertices);
                faces.Add(face);
            }
        }

        return faces;        
    }
    public abstract IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0);

    protected IReadOnlyList<Face> ResizeCore(GridTopology newGridTopology, int rows, int cols, int baseRow, int baseCol)
    {
        int offsetX = baseCol * UnitX;
        int offsetY = baseRow * UnitY;

        var newFaces = newGridTopology.Build(rows, cols);

        _topologyBuilder.Offset(offsetX, offsetY);

        newGridTopology.Builder.CopyAttributesFrom(_topologyBuilder, faceComparator: (f1, f2) =>
        {
            var (r1, c1) = GetGridFaceRowCol(f1);
            var (r2, c2) = GetGridFaceRowCol(f2);

            return r1 == r2 && c1 == c2;
        });

        _topologyBuilder = newGridTopology.Builder;
        return newFaces;
    }
    protected abstract IReadOnlyList<VertexKey> CreateGridFaceVertices(int row, int col);
    public abstract (int row, int col) GetGridFaceRowCol(Face face);
}
