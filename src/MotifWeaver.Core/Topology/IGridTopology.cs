using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public interface IGridTopology
{
    int UnitX { get; }
    int UnitY { get; }
    int Row { get; }
    int Col { get; }
    IReadOnlyList<Face> Build(int rows, int cols);
    int CalculateLogicalWidth();
    int CalculateLogicalHeight();
    IReadOnlyList<Face> Resize(int rows, int cols, int baseRow = 0, int baseCol = 0);
}
