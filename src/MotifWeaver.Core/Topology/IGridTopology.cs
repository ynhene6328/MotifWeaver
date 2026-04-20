using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public interface IGridTopology
{
    IReadOnlyList<Face> Build(int rows, int cols);

    (int width, int height) CalculateSize(IReadOnlyList<Face> faces);
}
