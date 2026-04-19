using System;
using System.Collections.Generic;

namespace MotifWeaver.Core.Topology;

public sealed class Pattern
{
    public IReadOnlyList<Face> Faces { get; }
    public int Rows { get; }
    public int Cols { get; }

    public Pattern(IReadOnlyList<Face> faces, int rows, int cols)
    {
        if (faces is null)
        {
            throw new ArgumentNullException(nameof(faces));
        }

        if (rows < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(rows));
        }

        if (cols < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cols));
        }

        Faces = faces;
        Rows = rows;
        Cols = cols;
    }
}
