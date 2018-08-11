using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores square locations in an integer grid.
/// </summary>
public class Build
{
    /// <summary>
    /// The vectors in this are expcted in the format (int, int, 0)
    /// TO TO: implement something enforcing this.
    /// </summary>
    public Dictionary<Vector3, SquareType> Squares { get; private set; }

    public Build()
    {
        Squares = new Dictionary<Vector3, SquareType>();
    }
}

