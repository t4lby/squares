using System;
using System.Collections.Generic;

/// <summary>
/// An inventory storing an amount of each square type.
/// </summary>
public class Inventory
{
    /// <summary>
    /// Initializes a new empty instance of the <see cref="T:Inventory"/> class.
    /// </summary>
    public Inventory()
    {
        Squares = new Dictionary<SquareType, int>();
        foreach (SquareType color in Enum.GetValues(typeof(SquareType)))
        {
            Squares.Add(color, 0);
        }
    }

    /// <summary>
    /// Stores amounts of each square type.
    /// </summary>
    public Dictionary<SquareType, int> Squares { get; private set; }
}
