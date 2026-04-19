using System.Collections.Generic;
using System.Numerics;
using MotifWeaver.Core.Geometry;

namespace MotifWeaver.Core.Topology;

public interface ITopologyQuery
{
    Face? FindFace(
        IReadOnlyList<Face> faces,
        Vector2 logicalPosition,
        IGridGeometry geometry);
}
