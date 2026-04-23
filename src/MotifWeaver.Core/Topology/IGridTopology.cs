using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public interface IGridTopology
{
    int UnitX { get; }
    int UnitY { get; }
    int Row { get; }
    int Col { get; }
    int LogicalWidth { get; }
    int LogicalHeight { get; }
    int MaxWidth { get; }
    int MaxHeight { get; }
    IReadOnlyList<Face> Build(int rows, int cols);
    IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0);
}
