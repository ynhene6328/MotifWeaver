using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MotifWeaver.Core.Geometry;

namespace MotifWeaver.Core.Topology;

public sealed class RayCastingTopologyQuery : ITopologyQuery
{
    public Face? FindFace(
        IReadOnlyList<Face> faces,
        Vector2 logicalPosition,
        IGridGeometry geometry)
    {
        if (faces is null)
        {
            throw new ArgumentNullException(nameof(faces));
        }

        if (geometry is null)
        {
            throw new ArgumentNullException(nameof(geometry));
        }

        for (int faceIndex = 0; faceIndex < faces.Count; faceIndex++)
        {
            Face face = faces[faceIndex];

            // BoundingBoxで事前フィルタ
            IEnumerable<VertexKey> keys = face.Vertices.Select(v => v.Key);
            BoundingBox bounds = geometry.ComputeBounds(keys);

            if (logicalPosition.X < bounds.MinX || logicalPosition.X > bounds.MaxX ||
                logicalPosition.Y < bounds.MinY || logicalPosition.Y > bounds.MaxY)
            {
                continue;
            }

            // Ray Castingによる判定
            IReadOnlyList<Vertex> vertices = face.Vertices;
            List<Vector2> polygon = new List<Vector2>(vertices.Count);
            for (int i = 0; i < vertices.Count; i++)
            {
                polygon.Add(geometry.GetPosition(vertices[i].Key));
            }

            if (IsPointInPolygon(logicalPosition, polygon))
            {
                return face;
            }
        }

        return null;
    }

    private static bool IsPointInPolygon(Vector2 point, IReadOnlyList<Vector2> polygon)
    {
        bool inside = false;
        int count = polygon.Count;

        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            Vector2 pi = polygon[i];
            Vector2 pj = polygon[j];

            if (((pi.Y > point.Y) != (pj.Y > point.Y)) &&
                (point.X < (pj.X - pi.X) * (point.Y - pi.Y) / (pj.Y - pi.Y) + pi.X))
            {
                inside = !inside;
            }
        }

        return inside;
    }
}
