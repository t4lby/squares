using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class GreenSquare : Square
{
    protected override void SetSquareProperties()
    {
        this.Health = 1f;
        this.Durability = 10f;
        this.Color = SquareType.Green;
        this.MinimumTransparency = 0.25f;
        this.RegenerationSpeed = 0.1f;
    }
}
