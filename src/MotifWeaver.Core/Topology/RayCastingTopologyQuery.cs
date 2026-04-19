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
        Vector2 logicalPosition)
    {
        if (faces is null)
        {
            throw new ArgumentNullException(nameof(faces));
        }

        for (int faceIndex = 0; faceIndex < faces.Count; faceIndex++)
        {
            Face face = faces[faceIndex];

            // 論理座標のBoundingBoxで事前フィルタ
            IEnumerable<VertexKey> keys = face.Vertices.Select(v => v.Key);
            var bounds = new
            {
                MinX = keys.Select(k => k.X).Min(),
                MaxX = keys.Select(k => k.X).Max(),
                MinY = keys.Select(k => k.Y).Min(),
                MaxY = keys.Select(k => k.Y).Max()
            };

            if (logicalPosition.X < bounds.MinX || logicalPosition.X > bounds.MaxX ||
                logicalPosition.Y < bounds.MinY || logicalPosition.Y > bounds.MaxY)
            {
                continue;
            }

            // Ray Castingによる判定
            var polygon = face.Vertices.Select(v => new Vector2(v.Key.X, v.Key.Y)).ToArray();

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
