using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

/// <summary>
/// Stores square locations in an integer grid.
/// </summary>
public class Build
{
    public Dictionary<Vector3, SquareType> Squares { get; set; }

    /// <summary>
    /// Stores the mappings for each square.
    /// </summary>
    public Dictionary<Vector3, KeyCode> Mappings { get; set; }

    /// <summary>
    ///Stores the rotation for each square
    /// </summary>
    public Dictionary<Vector3, Quaternion> Rotations { get; set; }

    public Build()
    {
        Squares = new Dictionary<Vector3, SquareType>();
        Mappings = new Dictionary<Vector3, KeyCode>();
        Rotations = new Dictionary<Vector3, Quaternion>();
    }
}

