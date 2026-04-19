using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public interface IGridTopology
{
    IReadOnlyList<Face> Build(int rows, int cols);

    (int rows, int cols) CalculateSize(IReadOnlyList<Face> faces);
}
